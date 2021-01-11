using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : MonoBehaviour
{
    public float lengthRelatedToCaliber;
    public float calibre;
    public Vector2 velocity;
    public float initialVelocity;
    public float drag;
    public bool execute;
    Vector3 heading;
    public float penetrationBase;
    public float velocityPenetrationLoss; // 1 = full velocity based; 0 = penetrationBase based;

    public LayerMask layerMask;
    public TrailRenderer trail;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (execute)
        {
            ExecuteTimestep(Time.deltaTime);
        }
    }
    public void FireShell(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        float angleOfFire = 90 - Vector3.Angle(direction, Vector3.up);
        print(angleOfFire);
        float horizontal = Mathf.Cos(Mathf.Deg2Rad * angleOfFire) * initialVelocity;
        float vertical = Mathf.Sin(Mathf.Deg2Rad * angleOfFire) * initialVelocity;
        print(horizontal);
        print(vertical);
        velocity.x = horizontal;
        velocity.y = vertical;
        execute = true;
        heading = new Vector3(direction.x,0,direction.z);
        heading = heading.normalized;
        Debug.DrawLine(transform.position, transform.position + heading, Color.yellow);
    }
    public void ExecuteTimestep(float timestep)
    {
        Vector3 pos = transform.position;
        Vector3 initialPos = pos ;
        pos.y += velocity.y * timestep;
        pos += heading * velocity.x * timestep;
        RaycastHit onehit;
        
        if(Physics.SphereCast(initialPos, calibre * 0.01f, pos - initialPos, out onehit, velocity.magnitude * timestep, layerMask))
        {
            float deltaAngle = Vector3.Angle(onehit.normal, initialPos - pos);

            if (deltaAngle > 69)
            {
                //bounce
                transform.position = onehit.point;
                Vector3 reflectedDir = Vector3.Reflect(pos-initialPos, onehit.normal);
                transform.rotation = Quaternion.LookRotation(reflectedDir, Vector3.up);
                float angleOfFire = 90 - Vector3.Angle(reflectedDir, Vector3.up);
                print(angleOfFire);
                float horizontal = Mathf.Cos(Mathf.Deg2Rad * angleOfFire) * velocity.magnitude * 0.85f;
                float vertical = Mathf.Sin(Mathf.Deg2Rad * angleOfFire) * velocity.magnitude * 0.85f;
                print(horizontal);
                print(vertical);
                velocity.x = horizontal;
                velocity.y = vertical;
                execute = true;
                heading = new Vector3(reflectedDir.x, 0, reflectedDir.z);
                heading = heading.normalized;
                Debug.DrawLine(transform.position, transform.position + heading, Color.yellow);

            }
            else {
                TankCollionData firstHit = onehit.collider.gameObject.GetComponent<TankCollionData>();
                float effectiveThickness = firstHit.thickness / Mathf.Cos(Mathf.Deg2Rad*deltaAngle);
                print(effectiveThickness);
                print(firstHit.thickness);
                float penetrationForce = Mathf.Lerp(penetrationBase, velocity.magnitude / initialVelocity * penetrationBase, velocityPenetrationLoss);
                print(penetrationBase);
                pos = onehit.point;
                transform.rotation = Quaternion.LookRotation(pos - initialPos, Vector3.up);
                transform.position = pos;
                execute = false;
                if (effectiveThickness < penetrationForce)
                {
                    print("HIT FIRST");
                    firstHit.hit = true;
                    penetrationForce -= effectiveThickness;
                    RaycastHit[] hit;
                    hit = Physics.SphereCastAll(pos, calibre * 0.01f, heading, 5, layerMask);
                    if (hit.Length != 0)
                    {
                        float[] distances = new float[hit.Length];
                        int[] pointers = new int[hit.Length];
                        for (int i = 0; i < hit.Length; i++)
                        {
                            distances[i] = hit[i].distance;
                        }
                        Array.Sort(distances);
                        for (int i = 0; i < hit.Length; i++)
                        {
                            for (int k = 0; k < hit.Length; k++)
                            {
                                if (distances[i] == hit[k].distance)
                                {
                                    pointers[i] = k;
                                }
                            }
                        }
                        for (int i = 0; i < hit.Length; i++)
                        {
                            TankCollionData nextHit = hit[i].collider.GetComponent<TankCollionData>();
                            if(nextHit.thickness < penetrationForce)
                            {
                                penetrationForce -= nextHit.thickness;
                                nextHit.hit = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                
            }

            

        }
        else
        {
            velocity.y -= 9.8f * timestep;
            velocity.x *= (1 - drag * timestep);

            transform.rotation = Quaternion.LookRotation(pos - initialPos, Vector3.up);
            transform.position = pos;
        }




    }
    public Vector3 RepairHitSurfaceNormal(RaycastHit hit, int layerMask)
    {
        if (hit.collider is MeshCollider)
        {
            var collider = hit.collider as MeshCollider;
            var mesh = collider.sharedMesh;
            var tris = mesh.triangles;
            var verts = mesh.vertices;

            var v0 = verts[tris[hit.triangleIndex * 3]];
            var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
            var v2 = verts[tris[hit.triangleIndex * 3 + 2]];

            var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;

            return hit.transform.TransformDirection(n);
        }
        else
        {
            var p = hit.point + hit.normal * 0.01f;
            Physics.Raycast(p, -hit.normal, out hit, 0.011f, layerMask);
            return hit.normal;
        }
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMain : MonoBehaviour
{
    public float heightOffset;
    public float scrollSpeed;
    public float scrollLerp;
    public float hAngle;
    public float vAngle;
    public float zDist;
    public float sensitivity;
    public Camera cam;
    public GameObject empty;
    Vector3 lastMousePos;
    public TankTrack leftTrack;
    public TankTrack rightTrack;
    public float engineTorque;
    public Transform turretPivot;
    public Transform barrelPivot;
    public Transform cameraPivot;
    public float turretAngle;
    public float barrelAngle;
    public Transform cube;
    public LayerMask layerMask;
    public float lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        hAngle += (mousePos.x - lastMousePos.x)*sensitivity;
        vAngle -= (mousePos.y - lastMousePos.y) * sensitivity;
        vAngle = Mathf.Clamp(vAngle, -45, 45);
        turretAngle = Mathf.Lerp(turretAngle, hAngle, Time.deltaTime * lerpSpeed);
        barrelAngle = Mathf.Lerp(barrelAngle, Mathf.Clamp(vAngle, -20, 5), Time.deltaTime * lerpSpeed); 
        turretPivot.localRotation = Quaternion.AngleAxis(turretAngle, Vector3.up);
        barrelPivot.localRotation = Quaternion.AngleAxis(barrelAngle, Vector3.right);
        cameraPivot.transform.localPosition = new Vector3(0, heightOffset, 0);
        cameraPivot.localRotation = Quaternion.AngleAxis(hAngle, Vector3.up)* Quaternion.AngleAxis(vAngle, Vector3.right);
        cam.transform.localPosition = new Vector3(0, 0, -zDist);
        float leftThrottle = Input.GetAxis("Vertical") + Input.GetAxis("Horizontal");
        float rightThrottle = Input.GetAxis("Vertical") - Input.GetAxis("Horizontal");
        Ray ray = new Ray(barrelPivot.position, barrelPivot.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit, layerMask))
        {
            Vector3 barrelDir = hit.point - turretPivot.position;
            cube.transform.position = hit.point;
            float deltaAngleVert = Vector3.SignedAngle(barrelPivot.transform.forward, barrelDir, -Vector3.right);
            float deltaAngleHorz = Vector3.SignedAngle(turretPivot.position+transform.forward, barrelDir, Vector3.up);


        }
        leftTrack.SetThrottle(leftThrottle * engineTorque);
        rightTrack.SetThrottle(rightThrottle * engineTorque);
        lastMousePos = Input.mousePosition;

    }
    void MoveCamera()
    {
        Vector3 mousePos = Input.mousePosition;

        hAngle += mousePos.x - lastMousePos.x;

        Vector3 cameraPos;
        Vector3 offsetCentre = transform.position;
        offsetCentre.y += heightOffset;
        empty.transform.rotation = Quaternion.AngleAxis(hAngle, Vector3.up)* Quaternion.AngleAxis(vAngle, Vector3.right);

        Vector3 direction = empty.transform.TransformDirection(new Vector3(zDist,0,0));
        
        cameraPos = offsetCentre + direction * zDist;
        cam.transform.position = cameraPos;
        cam.transform.rotation = Quaternion.LookRotation(cameraPos - offsetCentre, Vector3.up);
        lastMousePos = Input.mousePosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWheel : MonoBehaviour
{
    public WheelCollider collider;
    public Quaternion quat;
    public bool colliderLess;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!colliderLess)
        {
            Vector3 pos;
            Quaternion temp;
            collider.GetWorldPose(out pos, out temp);
            transform.position = pos;
        }
    }
}

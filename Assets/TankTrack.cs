using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrack : MonoBehaviour
{
    public TankWheel[] tankWheels;
    public float rotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetThrottle(float throttle)
    {
        for (int i = 0; i < tankWheels.Length; i++)
        {
            if (!tankWheels[i].colliderLess)
            {
                tankWheels[i].collider.motorTorque = throttle;
            }
        }
    }
    public void SetSteering(float steering)
    {
        for (int i = 0; i < tankWheels.Length; i++)
        {
            if (!tankWheels[i].colliderLess)
            {
                tankWheels[i].collider.steerAngle = steering;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        float average = 0;
        int counter = 0;
        for (int i = 0; i < tankWheels.Length; i++)
        {
            if (!tankWheels[i].colliderLess)
            {
                average += tankWheels[i].collider.rpm;
                counter++;
            }
        }
        //average /= tankWheels.Length;
        average *= Time.deltaTime;
        rotation += average;
        for (int i = 0; i < tankWheels.Length; i++)
        {
            TankWheel tw = tankWheels[i];
            tw.transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.right);
        }
    }
}

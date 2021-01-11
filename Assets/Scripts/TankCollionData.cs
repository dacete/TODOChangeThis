using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCollionData : MonoBehaviour
{
    public float thickness;
    public bool hit;
}
public enum TankColType
{
    TankTread,
    ExternalModule,
    InternalModule,
    Crew
}

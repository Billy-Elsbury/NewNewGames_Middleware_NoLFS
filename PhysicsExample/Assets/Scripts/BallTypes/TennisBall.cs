
using UnityEngine;

public class TennisBall : SpherePhysics
{
    void Awake()
    {
        mass = 1f;
        coefficientOfRestitution = 0.4f;
    }
}

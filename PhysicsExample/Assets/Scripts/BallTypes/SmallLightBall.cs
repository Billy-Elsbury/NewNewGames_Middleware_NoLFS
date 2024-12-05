
using UnityEngine;

public class TennisBall : SpherePhysics
{
    void Awake()
    {
        mass = 0.3f;
        coefficientOfRestitution = 0.4f;
    }
}

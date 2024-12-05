
using UnityEngine;

public class BeachBall : SpherePhysics
{
    void Awake()
    {
        mass = 1f;
        coefficientOfRestitution = 0.7f; 
    }
}

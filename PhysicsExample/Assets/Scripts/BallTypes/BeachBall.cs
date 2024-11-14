
using UnityEngine;

public class BeachBall : SpherePhysics
{
    void Awake()
    {
        mass = 2f;
        coefficientOfRestitution = 0.7f; 
    }
}

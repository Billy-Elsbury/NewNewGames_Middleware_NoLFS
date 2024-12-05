
using UnityEngine;

public class BowlingBall : SpherePhysics
{
    void Awake()
    {
        mass = 7.0f; 
        coefficientOfRestitution = 0.3f; 
    }
}

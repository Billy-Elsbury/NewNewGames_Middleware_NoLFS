
using UnityEngine;

public class GolfBall : SpherePhysics
{
    void Awake()
    {
        mass = 1f;
        coefficientOfRestitution = 0.4f;
    }
}


using UnityEngine;

public class GolfBall : SpherePhysics
{
    void Awake()
    {
        mass = 0.5f;
        coefficientOfRestitution = 0.4f;
    }
}

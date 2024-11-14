
using UnityEngine;

public class PoolBall : SpherePhysics
{
    void Awake()
    {
        mass = 1f;
        coefficientOfRestitution = 0.4f;
    }
}

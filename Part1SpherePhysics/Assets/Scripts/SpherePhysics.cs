using UnityEngine;

public class SpherePhysics : MonoBehaviour
{
    internal Vector3 previousVelocity, previousPosition, acceleration;
    internal Vector3 velocity;
    internal float mass = 10.0f;
    internal float gravity = 9.81f;
    internal float spaceGravity = 0.01f;
    internal float coefficientOfRestitution = 0.8f;
    internal float drag = 0.3f;
    internal float maxAcceleration = 1f;

    public float Radius
    {
        get { return transform.localScale.x / 2.0f; }
        private set { transform.localScale = value * 2 * Vector3.one; }
    }

    // Update is called once per frame
    internal void FixedUpdate()
    {
        previousVelocity = velocity;
        previousPosition = transform.position;

        // Calculate forces without directly overwriting acceleration
        Vector3 gravityForce = spaceGravity * Vector3.down;

        Vector3 dragForce = -drag * velocity;
        if (dragForce.magnitude > velocity.magnitude * mass)
            dragForce = velocity.normalized * velocity.magnitude * mass;

        acceleration += dragForce / mass;

        // add gravity after player input
        velocity += (acceleration + gravityForce) * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;

        //reset
        acceleration = Vector3.zero;
    }



    public void ResolveCollisionWith(PlaneScript planeScript)
    {
        float currentDistance = planeScript.distanceFromSphere(this);
        float previousDistance = Vector3.Dot(previousPosition - planeScript.Position, planeScript.Normal) - Radius;

        // DEBUG
        //print("Distance: " + currentDistance + " Old Distance: " + previousDistance);

        // Step 1) To check dividing by zero
        float timeOfImpact = -previousDistance / (currentDistance - previousDistance) * Time.deltaTime;
        // DEBUG print("TOI: " + timeOfImpact + " deltaTime: " + Time.deltaTime);

        // Step 2)
        Vector3 positionOfImpact = previousPosition + (timeOfImpact * velocity);

        // Recalculate velocity using timeOfImpact
        Vector3 velocityAtImpact = previousVelocity + (acceleration * timeOfImpact);

        // Step 3) Resolve Collision
        Vector3 normalComponent = Utility.ProjectVectorOntoNormal(velocityAtImpact, planeScript.Normal);
        Vector3 perpendicularComponent = Utility.ExtractComponentPerpendicularToNormal(velocityAtImpact, planeScript.Normal);

        Vector3 newVelocity = (perpendicularComponent - coefficientOfRestitution * normalComponent);

        // Calculate remaining time after impact
        float timeRemaining = Time.deltaTime - timeOfImpact;

        velocity = newVelocity + acceleration * timeRemaining;

        // Check velocity is moving ball away from plane (IE same direction as normal +- 90 degrees)
        if (Vector3.Dot(velocity, planeScript.Normal) < 0)
        {
            velocity = Utility.ExtractComponentPerpendicularToNormal(velocity, planeScript.Normal);
        }

        transform.position = positionOfImpact + velocity * timeRemaining;
    }

    public bool isCollidingWith(SpherePhysics otherSphere)
    {
        return Vector3.Distance(otherSphere.transform.position, transform.position) < (otherSphere.Radius + Radius);
    }

    public void ResolveCollisionWith(SpherePhysics sphere2)
    {
        // Calculate time of impact
        float currentSpherePlaneDistance = Vector3.Distance(sphere2.transform.position, transform.position) - (sphere2.Radius + Radius);
        float previousSpherePlaneDistance = Vector3.Distance(sphere2.previousPosition, previousPosition) - (sphere2.Radius + Radius);

        float timeOfImpact = -previousSpherePlaneDistance / (currentSpherePlaneDistance - previousSpherePlaneDistance) * Time.deltaTime;
        //print("TOI: " + timeOfImpact + " deltaTime: " + Time.deltaTime);

        // After getting TOI, calculate position of spheres at impact for both spheres.
        Vector3 sphere1AtImpact = previousPosition + velocity * timeOfImpact;
        Vector3 sphere2AtImpact = sphere2.previousPosition + sphere2.velocity * timeOfImpact;

        // Recalculate Velocity for both spheres from previous position, but using timeOfImpact instead of deltaTime
        Vector3 Sphere1VelocityAtImpact = previousVelocity + (acceleration * timeOfImpact);
        Vector3 sphere2VelocityAtImpact = sphere2.previousVelocity + (sphere2.acceleration * timeOfImpact);

        // Normal of collision at Time of Impact
        Vector3 collisionNormal = (sphere1AtImpact - sphere2AtImpact).normalized;

        Vector3 sphere1ParallelToNormal = Utility.ProjectVectorOntoNormal(Sphere1VelocityAtImpact, collisionNormal);
        Vector3 sphere1PerpendicularToNormal = Utility.ExtractComponentPerpendicularToNormal(Sphere1VelocityAtImpact, collisionNormal);
        Vector3 sphere2ParallelToNormal = Utility.ProjectVectorOntoNormal(sphere2VelocityAtImpact, collisionNormal);
        Vector3 sphere2PerpendicularToNormal = Utility.ExtractComponentPerpendicularToNormal(sphere2VelocityAtImpact, collisionNormal);

        Vector3 prevParallelVelocity1 = sphere1ParallelToNormal;
        Vector3 prevParallelVelocity2 = sphere2ParallelToNormal;

        // Velocities after impact parallel to the normal
        Vector3 parallelVelocity1 = ((mass - sphere2.mass) / (mass + sphere2.mass)) * prevParallelVelocity1 + ((sphere2.mass * 2) / (mass + sphere2.mass)) * prevParallelVelocity2;
        Vector3 parallelVelocity2 = (-(mass - sphere2.mass) / (mass + sphere2.mass)) * prevParallelVelocity2 + ((mass * 2) / (mass + sphere2.mass)) * prevParallelVelocity1;

        velocity = sphere1PerpendicularToNormal + parallelVelocity1 * coefficientOfRestitution;
        Vector3 sphere1VelocityAfterImpact = sphere1PerpendicularToNormal + parallelVelocity1 * coefficientOfRestitution;
        Vector3 sphere2VelocityAfterImpact = sphere2PerpendicularToNormal + parallelVelocity2 * coefficientOfRestitution;

        // Calculate velocity from impact time to time of detection (remaining time after impact)
        float timeRemaining = Time.deltaTime - timeOfImpact;

        velocity = sphere1VelocityAfterImpact + acceleration * timeRemaining;
        Vector3 sphere2Velocity = sphere2VelocityAfterImpact + sphere2.acceleration * timeRemaining;

        // Update this sphere first
        transform.position = sphere1AtImpact + sphere1VelocityAfterImpact * timeRemaining;

        // Calculate other sphere position
        Vector3 sphere2ResolvedPosition = sphere2AtImpact + sphere2VelocityAfterImpact * timeRemaining;

        // Checking for overlap between spheres after resolution
        if (Vector3.Distance(transform.position, sphere2ResolvedPosition) < (Radius + sphere2.Radius))
        {
            print("Overlap Detected");
        }

        sphere2.slaveCollisionResolution(sphere2ResolvedPosition, sphere2Velocity);
    }

    private void slaveCollisionResolution(Vector3 position, Vector3 newVelocity)
    {
        transform.position = position;
        velocity = newVelocity;
    }
}

using UnityEngine;

public class PlayerBallMovement : SpherePhysics
{
    public float movementSpeed = 1f;
    public Transform cameraTransform;

    // Update is called once per frame
    void Update()
    {
        acceleration = Vector3.zero;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.Normalize();
        right.Normalize();

        if (Input.GetKey(KeyCode.W))
            acceleration += forward * movementSpeed;

        if (Input.GetKey(KeyCode.S))
            acceleration += -forward * movementSpeed;

        if (Input.GetKey(KeyCode.A))
            acceleration += -right * movementSpeed;

        if (Input.GetKey(KeyCode.D))
            acceleration += right * movementSpeed;

        base.Update();
    }
}
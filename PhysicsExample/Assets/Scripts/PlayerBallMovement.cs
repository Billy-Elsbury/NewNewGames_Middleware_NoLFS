using UnityEngine;

public class PlayerBallMovement : SpherePhysics
{
    public float movementSpeed = 100f;
    public Transform cameraTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        base.FixedUpdate();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.Normalize();
        right.Normalize();

        Vector3 movementInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            movementInput += forward * movementSpeed;

        if (Input.GetKey(KeyCode.S))
            movementInput += -forward * movementSpeed;

        if (Input.GetKey(KeyCode.A))
            movementInput += -right * movementSpeed;

        if (Input.GetKey(KeyCode.D))
            movementInput += right * movementSpeed;

        velocity += movementInput * Time.deltaTime;
    }


}
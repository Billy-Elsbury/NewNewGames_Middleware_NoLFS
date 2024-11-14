using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      
    public float distance = 5.0f;        
    public float rotationSpeed = 5.0f;    

    private float yaw = 0.0f;           
    private float pitch = 0.0f;          

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp the pitch to prevent flipping
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.position = position;
        transform.LookAt(target);
    }
}

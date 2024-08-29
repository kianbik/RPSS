using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;         // Reference to the player character's transform
    public Vector3 offset;           // Offset from the player to the camera
    public float rotationSpeed = 5f; // Speed of camera rotation
    public float minPitch = -40f;    // Minimum pitch angle (look down limit)
    public float maxPitch = 80f;     // Maximum pitch angle (look up limit)

    private float pitch = 0f;        // Vertical angle
    private float yaw = 0f;          // Horizontal angle

    void Start()
    {
        // Initialize the camera's position with the given offset
        transform.position = player.position + offset;
    }

    void LateUpdate()
    {
        // Get mouse input for camera rotation
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp the pitch to avoid flipping the camera too far up or down
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Rotate the camera around the player based on mouse input
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.position + rotation * offset;

        // Always look at the player
        transform.LookAt(player.position + Vector3.up * offset.y);
    }
}


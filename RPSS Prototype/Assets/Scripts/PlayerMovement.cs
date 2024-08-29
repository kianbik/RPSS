using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;       // Reference to the camera's transform
    public Transform player;                // Reference to the player's transform
    public Vector3 offset;                  // Offset from the player to the camera
    public float walkSpeed = 5f;            // Normal walking speed
    public float sprintSpeed = 10f;         // Sprinting speed
    public float jumpHeight = 2f;           // Jump height
    public float gravity = -9.81f;          // Gravity affecting the player

    public float rotationSpeed = 5f;        // Speed of camera rotation
    public float minPitch = -40f;           // Minimum pitch angle (look down limit)
    public float maxPitch = 80f;            // Maximum pitch angle (look up limit)
    public float playerRotationSpeed = 10f; // Speed at which the player rotates towards the camera direction

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float pitch = 0f;               // Vertical angle for the camera
    private float yaw = 0f;                 // Horizontal angle for the camera

    void Start()
    {
        controller = GetComponent<CharacterController>();
        yaw = cameraTransform.eulerAngles.y;  // Initialize the yaw to the camera's initial rotation
    }

    void LateUpdate()
    {
        // Handle camera rotation
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Rotate the camera around the player
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        cameraTransform.position = player.position + rotation * offset;

        // Make the camera look at the player
        cameraTransform.LookAt(player.position + Vector3.up * offset.y);

        // Handle character movement
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep the player grounded
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate the direction the character should move based on the camera's direction
        Vector3 move = cameraTransform.right * moveX + cameraTransform.forward * Mathf.Abs(moveZ);
        move.y = 0f; // Ensure we don't move the character vertically

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Rotate the player to face the camera's forward direction (only Y-axis rotation)
        if (move != Vector3.zero)
        {
            // Extract the Y-axis rotation from the camera's forward direction
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f; // Remove any vertical component
            forward.Normalize(); // Normalize to get the forward direction on the XZ plane

            Quaternion targetRotation = Quaternion.LookRotation(forward);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * playerRotationSpeed);
        }

        // Move the character (moving backward while facing forward if moveZ < 0)
        controller.Move(move * Mathf.Sign(moveZ) * speed * Time.deltaTime);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
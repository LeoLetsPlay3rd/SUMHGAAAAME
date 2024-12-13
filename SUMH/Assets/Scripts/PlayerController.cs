using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f; // Walking speed
    public float jumpForce = 5.0f; // Jumping force
    public float gravity = -9.81f; // Gravity applied to the player

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f; // Controls mouse sensitivity

    [Header("References")]
    public CharacterController controller; // Reference to the CharacterController
    public Transform cameraTransform; // Reference to the camera transform

    private Vector3 velocity; // Tracks the player's vertical velocity
    private float xRotation = 0f; // Tracks the vertical rotation of the camera

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player body horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically, clamping to avoid looking too far up or down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limits vertical rotation
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // Get input for movement
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right keys
        float moveZ = Input.GetAxis("Vertical"); // W/S or Up/Down keys

        // Move relative to the player's current direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep the player grounded
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Jumping
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
}

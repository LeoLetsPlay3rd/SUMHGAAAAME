using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;

    [Header("Mouse/Controller Settings")]
    public float sensitivity = 100f;

    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform;

    private Vector3 velocity;
    private float xRotation = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction(); // Check for interactions each frame
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    // Handles camera movement
    void HandleLook()
    {
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Handles player movement and gravity
    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Handles interaction logic
    void HandleInteraction()
    {
        // Check for keyboard input
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Interacted with Keyboard X!");
            Interact();
        }

        // Check for PS5 controller input
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log("Interacted with PS5 Controller X (Button South)!");
            Interact();
        }
    }

    // Interaction logic
    void Interact()
    {
        // Replace this with your game's interaction logic
        Debug.Log("Performing interaction...");
    }
}

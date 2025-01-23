using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class StartingSceneManager : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage; // Reference to the fade image in the scene
    public float fadeDuration = 1f; // Duration of the fade effect

    [Header("Look Around Settings")]
    public Transform playerCamera; // Reference to the player's camera
    public Vector2 lookConstraints = new Vector2(45f, 45f); // Look constraints (vertical, horizontal)
    public float lookSensitivity = 2f; // Sensitivity for looking around

    private Vector2 currentRotation; // Tracks the current rotation of the camera

    private void Start()
    {
        // Ensure the fade image starts fully black, then fade in
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
    }

    private void Update()
    {
        // Get input from the right joystick or mouse movement
        Vector2 lookInput = Vector2.zero;
        if (Gamepad.current != null)
        {
            lookInput = Gamepad.current.rightStick.ReadValue();
        }
        else
        {
            lookInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        }

        // Pass the input to HandleLookAround
        HandleLookAround(lookInput);

        // Check for the "X" button press (keyboard or controller)
        if (Input.GetKeyDown(KeyCode.X) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
        {
            Debug.Log("Player pressed X. Transitioning to the next scene.");
            StartCoroutine(TransitionToNextScene(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    private void HandleLookAround(Vector2 input)
    {
        if (playerCamera != null)
        {
            // Invert the vertical input (Y-axis) for correct look behavior
            input.y = -input.y;

            // Adjust the rotation based on sensitivity and constraints
            float upwardLimit = lookConstraints.x + 12f; // Allow more upward range
            currentRotation.x = Mathf.Clamp(currentRotation.x + input.y * lookSensitivity, -lookConstraints.x, upwardLimit);
            currentRotation.y = Mathf.Clamp(currentRotation.y + input.x * lookSensitivity, -lookConstraints.y, lookConstraints.y);

            // Apply the rotation to the camera
            playerCamera.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / fadeDuration);
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, alpha);
            }
            yield return null;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0); // Ensure fully transparent
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / fadeDuration;
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, alpha);
            }
            yield return null;
        }
    }

    private IEnumerator TransitionToNextScene(int nextSceneIndex)
    {
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeOut()); // Fade out to black
        }

        // Reset the player's camera rotation and position here, while the screen is black
        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.identity; // Reset camera rotation
            currentRotation = Vector2.zero; // Reset stored rotation
            Debug.Log("Camera rotation reset after fade-out.");
        }

        // Load the next scene
        SceneManager.LoadScene(nextSceneIndex);
    }
}

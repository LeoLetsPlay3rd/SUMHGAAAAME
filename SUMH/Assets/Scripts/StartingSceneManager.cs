using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartingSceneManager : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage; // Reference to the fade image in the scene
    public float fadeDuration = 1f; // Duration of the fade effect

    [Header("Look Around Settings")]
    public Transform playerCamera; // Reference to the player's camera
    public Vector2 lookConstraints = new Vector2(45f, 45f); // Look constraints (vertical, horizontal)
    public float lookSensitivity = 2f; // Sensitivity for looking around

    [Header("Text Prompts")]
    public TextMeshProUGUI lookAroundText; // "Use (R) to look around" text
    public TextMeshProUGUI pressXText; // "Press (X) to get up" text
    public float textFadeDuration = 1f; // Duration of text fade-in
    public float textDisplayDelay = 2f; // Delay before the "Use (R)" text fades in

    private Vector2 currentRotation; // Tracks the current rotation of the camera
    private bool hasLookedAround = false; // Tracks if the player has looked around
    private bool pressXPromptShown = false; // Tracks if the "Press (X)" prompt has been shown

    private void Start()
    {
        // Ensure the fade image starts fully black, then fade in
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }

        // Start with the texts invisible
        if (lookAroundText != null)
        {
            lookAroundText.alpha = 0f; // Invisible at start
            lookAroundText.gameObject.SetActive(false); // Disable the object
        }
        if (pressXText != null)
        {
            pressXText.alpha = 0f; // Invisible at start
            pressXText.gameObject.SetActive(false); // Disable the object
        }

        // Start showing the "Use (R)" prompt after a delay
        StartCoroutine(ShowLookAroundPrompt());
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

        // Handle looking around
        if (lookInput != Vector2.zero)
        {
            HandleLookAround(lookInput);

            // If the player has looked around, disable the "Use (R)" prompt
            if (!hasLookedAround)
            {
                hasLookedAround = true;
                StopCoroutine(ShowLookAroundPrompt());
                if (lookAroundText != null)
                {
                    StartCoroutine(FadeOutTMPText(lookAroundText));
                }

                // Show the "Press (X)" prompt after looking around
                if (!pressXPromptShown)
                {
                    StartCoroutine(ShowPressXPrompt());
                }
            }
        }

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

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.identity; // Reset camera rotation
            currentRotation = Vector2.zero; // Reset stored rotation
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private IEnumerator ShowLookAroundPrompt()
    {
        yield return new WaitForSeconds(textDisplayDelay);

        if (!hasLookedAround && lookAroundText != null)
        {
            lookAroundText.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInTMPText(lookAroundText));
        }
    }

    private IEnumerator ShowPressXPrompt()
    {
        pressXPromptShown = true;

        // Wait 2 seconds after the "Use (R)" text fades out
        yield return new WaitForSeconds(2f);

        if (pressXText != null)
        {
            pressXText.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInTMPText(pressXText));
        }
    }

    private IEnumerator FadeInTMPText(TextMeshProUGUI text)
    {
        float elapsedTime = 0f;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text.alpha = Mathf.Lerp(0f, 1f, elapsedTime / textFadeDuration);
            yield return null;
        }

        text.alpha = 1f;
    }

    private IEnumerator FadeOutTMPText(TextMeshProUGUI text)
    {
        float elapsedTime = 0f;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text.alpha = Mathf.Lerp(1f, 0f, elapsedTime / textFadeDuration);
            yield return null;
        }

        text.alpha = 0f;
        text.gameObject.SetActive(false);
    }
}

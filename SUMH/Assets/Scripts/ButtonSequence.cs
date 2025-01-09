using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonSequence : MonoBehaviour
{
    public TextMeshProUGUI buttonText; // Reference to the button's text
    public TextMeshProUGUI pressText; // Reference to the "Press (X)" text
    public ButtonSequence nextButton; // Reference to the next button in the sequence
    public string nextSceneName; // Name of the next scene (only for the last button)
    public Image fadeOverlay; // Reference to the fade overlay (optional)
    public bool isFirstButton = false; // Flag to indicate if this is the first button
    public float loadingDelay = 2f; // Delay before the first button fades in (after loading)

    public float pressTextTimeout = 3f; // Time before "Press (X)" appears
    public float pressTextFadeDuration = 0.5f; // Duration of "Press (X)" fade-in

    private bool isClickable = false; // Tracks if the button is interactable
    private bool buttonClicked = false; // Tracks if the button has been clicked

    void OnEnable()
    {
        buttonClicked = false;
        isClickable = false;

        if (buttonText != null)
        {
            buttonText.alpha = 0f; // Start with the button text invisible
        }
        if (pressText != null)
        {
            pressText.alpha = 0f; // Start with "Press (X)" invisible
        }

        if (isFirstButton)
        {
            // For the first button, wait until everything is loaded
            StartCoroutine(WaitForLoadingAndFadeIn());
        }
        else
        {
            // For other buttons, fade in immediately
            StartCoroutine(FadeInText());
        }
    }

    IEnumerator WaitForLoadingAndFadeIn()
    {
        // Wait for the specified loading delay
        yield return new WaitForSeconds(loadingDelay);

        // Begin fading in the first button
        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {
        float fadeDuration = 1f; // Duration of the fade-in
        float elapsed = 0f;

        // Gradually fade in the button text
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        // Ensure the text is fully visible
        buttonText.alpha = 1f;

        // Calculate delay based on the text length
        float delayPerCharacter = 0.02f; // Delay per character in seconds
        int textLength = buttonText.text.Length; // Number of characters in the text
        float underlineDelay = textLength * delayPerCharacter; // Total delay
        yield return new WaitForSeconds(underlineDelay);

        // Add underline to the button text
        buttonText.fontStyle |= FontStyles.Underline;

        // Enable the button for interaction
        isClickable = true;

        // Start a timeout for showing "Press (X)" AFTER the button is fully visible
        StartCoroutine(ShowPressTextAfterTimeout());
    }

    IEnumerator ShowPressTextAfterTimeout()
    {
        // Wait for the timeout period only after the button has fully appeared
        float elapsed = 0f;
        while (elapsed < pressTextTimeout && !buttonClicked)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // If the button hasn't been clicked, fade in "Press (X)"
        if (!buttonClicked && pressText != null)
        {
            StartCoroutine(FadeInPressText());
        }
    }

    IEnumerator FadeInPressText()
    {
        float elapsed = 0f;

        // Gradually fade in the "Press (X)" text
        while (elapsed < pressTextFadeDuration)
        {
            elapsed += Time.deltaTime;
            pressText.alpha = Mathf.Lerp(0f, 1f, elapsed / pressTextFadeDuration);
            yield return null;
        }

        pressText.alpha = 1f; // Ensure full visibility
    }

    public void OnButtonClick()
    {
        if (!isClickable) return; // Ignore clicks if the button is not clickable

        buttonClicked = true; // Mark the button as clicked

        if (nextButton != null)
        {
            // Fade out the current button and activate the next one
            StartCoroutine(FadeOutAndActivateNext());
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Delegate scene transition to the SceneController
            SceneController sceneController = FindObjectOfType<SceneController>();
            if (sceneController != null)
            {
                sceneController.BeginSceneTransition(nextSceneName, fadeOverlay); // Pass both parameters
            }
            else
            {
                Debug.LogWarning("SceneController not found in the scene.");
            }
        }
    }

    IEnumerator FadeOutAndActivateNext()
    {
        float fadeDuration = 0.5f; // Duration of the fade-out
        float elapsed = 0f;

        // Gradually fade out the button text and "Press (X)"
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            if (pressText != null)
            {
                pressText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            }
            yield return null;
        }

        // Ensure the text is fully invisible
        buttonText.alpha = 0f;
        if (pressText != null)
        {
            pressText.alpha = 0f;
        }

        // Deactivate the current button
        gameObject.SetActive(false);

        // Activate the next button (if it exists)
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }
}

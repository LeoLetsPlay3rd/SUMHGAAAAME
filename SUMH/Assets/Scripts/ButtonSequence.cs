using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
            StartCoroutine(WaitForLoadingAndFadeIn());
        }
        else
        {
            StartCoroutine(FadeInText());
        }
    }

    void Update()
    {
        // Detect Gamepad X Button (South Button) or Keyboard X Key for button interaction
        if (isClickable && !buttonClicked &&
            ((Gamepad.current?.buttonSouth.wasPressedThisFrame == true) ||
             (Keyboard.current?.xKey.wasPressedThisFrame == true)))
        {
            OnButtonClick();
        }
    }

    IEnumerator WaitForLoadingAndFadeIn()
    {
        yield return new WaitForSeconds(loadingDelay);
        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        buttonText.alpha = 1f;

        float delayPerCharacter = 0.02f;
        float underlineDelay = buttonText.text.Length * delayPerCharacter;
        yield return new WaitForSeconds(underlineDelay);

        buttonText.fontStyle |= FontStyles.Underline;

        isClickable = true;

        StartCoroutine(ShowPressTextAfterTimeout());
    }

    IEnumerator ShowPressTextAfterTimeout()
    {
        float elapsed = 0f;
        while (elapsed < pressTextTimeout && !buttonClicked)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!buttonClicked && pressText != null)
        {
            StartCoroutine(FadeInPressText());
        }
    }

    IEnumerator FadeInPressText()
    {
        float elapsed = 0f;
        while (elapsed < pressTextFadeDuration)
        {
            elapsed += Time.deltaTime;
            pressText.alpha = Mathf.Lerp(0f, 1f, elapsed / pressTextFadeDuration);
            yield return null;
        }

        pressText.alpha = 1f;
    }

    public void OnButtonClick()
    {
        if (!isClickable) return;

        buttonClicked = true;

        if (nextButton != null)
        {
            StartCoroutine(FadeOutAndActivateNext());
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Delegate scene transition to SceneController
            SceneController sceneController = FindObjectOfType<SceneController>();
            if (sceneController != null)
            {
                // Let SceneController handle the fade and scene transition
                sceneController.BeginSceneTransition(nextSceneName);
            }
            else
            {
                Debug.LogWarning("SceneController not found in the scene.");
            }
        }
    }

    IEnumerator FadeOutAndActivateNext()
    {
        float fadeDuration = 0.5f;
        float elapsed = 0f;

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

        buttonText.alpha = 0f;
        if (pressText != null)
        {
            pressText.alpha = 0f;
        }

        gameObject.SetActive(false);

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController2 : MonoBehaviour
{
    private GameObject currentSelected; // The currently selected button
    private bool transitioningScene = false; // Flag for managing scene transitions

    public Image fadeOverlay; // Reference to the main fade overlay
    public Image secondaryFadeOverlay; // Reference to the secondary fade overlay (for MainMenu transitions)
    public float fadeDuration = 1f; // Duration of fade in/out effects

    private void Start()
    {
        // Ensure the fade overlay is fully black at the start
        if (fadeOverlay != null)
        {
            fadeOverlay.color = new Color(0f, 0f, 0f, 1f); // Fully opaque
            fadeOverlay.gameObject.SetActive(true); // Ensure it's active
            StartCoroutine(FadeInScene()); // Begin fade-in
        }
        else
        {
            Debug.LogWarning("SceneController2: No main fadeOverlay assigned!");
        }

        // Ensure the secondary fade overlay is disabled at the start
        if (secondaryFadeOverlay != null)
        {
            secondaryFadeOverlay.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("SceneController2: No secondary fadeOverlay assigned!");
        }
    }

    void Update()
    {
        if (transitioningScene) return; // Skip input handling during transitions

        // Dynamically update the selected button
        UpdateCurrentSelected();

        // Handle controller or keyboard input for triggering the current button
        if ((Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) || // "South" button is usually "X" on a controller
            (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame))        // "X" key on the keyboard
        {
            TriggerButtonClick();
        }
    }

    private void UpdateCurrentSelected()
    {
        // Get the currently selected GameObject from the EventSystem
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != currentSelected)
        {
            // If the selected object changes, update the reference
            currentSelected = selected;
        }

        // If no button is selected, find the first active and interactable button
        if (currentSelected == null || !currentSelected.activeInHierarchy || !currentSelected.GetComponent<Button>().interactable)
        {
            SetNextAvailableButton();
        }
    }

    private void SetNextAvailableButton()
    {
        // Find the first active and interactable button in the scene
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            if (button.gameObject.activeInHierarchy && button.interactable)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                currentSelected = button.gameObject;
                return;
            }
        }

        // No buttons found
        Debug.LogWarning("No enabled buttons found in the scene.");
    }

    private void TriggerButtonClick()
    {
        if (currentSelected != null)
        {
            // Trigger the OnClick event of the selected button
            Button button = currentSelected.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("No button is selected to trigger click.");
        }
    }

    public void BeginSceneTransition(string nextSceneName, Image fadeOverlayToUse)
    {
        if (transitioningScene) return; // Prevent multiple transitions
        transitioningScene = true;

        StartCoroutine(FadeOutSceneAndLoadNext(nextSceneName, fadeOverlayToUse));
    }

    private IEnumerator FadeOutSceneAndLoadNext(string nextSceneName, Image fadeOverlayToUse)
    {
        float elapsed = 0f;

        // Ensure the fade overlay is visible and starts transparent
        if (fadeOverlayToUse != null)
        {
            fadeOverlayToUse.gameObject.SetActive(true); // Activate the specified fade overlay
            Color overlayColor = fadeOverlayToUse.color;
            overlayColor.a = 0f; // Ensure starting alpha is 0
            fadeOverlayToUse.color = overlayColor;

            // Fade in the overlay to black
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration); // Gradually increase alpha
                fadeOverlayToUse.color = overlayColor;
                yield return null;
            }

            // Ensure fully black
            fadeOverlayToUse.color = new Color(0f, 0f, 0f, 1f);
            Debug.Log("SceneController2: Fade-out complete.");
        }

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);

        // Wait for the scene to load
        yield return new WaitForSeconds(0.1f);

        // Begin fade-in for the new scene
        if (fadeOverlayToUse == fadeOverlay)
        {
            StartCoroutine(FadeInScene());
        }
    }

    private IEnumerator FadeInScene()
    {
        float elapsed = 0f;

        // Gradually fade the overlay from black to transparent
        if (fadeOverlay != null)
        {
            Color overlayColor = fadeOverlay.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // Gradually decrease alpha
                fadeOverlay.color = overlayColor;
                yield return null;
            }

            // Ensure fully transparent
            fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
            fadeOverlay.gameObject.SetActive(false); // Deactivate the fade overlay
            Debug.Log("SceneController2: Fade-in complete.");
        }
    }

    /// <summary>
    /// Transitions to the MainMenu scene using the secondary fade overlay.
    /// </summary>
    public void TransitionToMainMenu()
    {
        if (!transitioningScene)
        {
            Debug.Log("SceneController2: Transitioning to MainMenu using secondary fade overlay...");
            BeginSceneTransition("MainMenu", secondaryFadeOverlay);
        }
    }
}

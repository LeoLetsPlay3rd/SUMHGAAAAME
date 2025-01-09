using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private GameObject currentSelected; // The currently selected button
    private bool transitioningScene = false; // Flag for managing scene transitions

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

    public void BeginSceneTransition(string nextSceneName, Image fadeOverlay)
    {
        if (transitioningScene) return; // Prevent multiple transitions
        transitioningScene = true;

        StartCoroutine(FadeOutSceneAndLoadNext(nextSceneName, fadeOverlay));
    }

    private IEnumerator FadeOutSceneAndLoadNext(string nextSceneName, Image fadeOverlay)
    {
        float fadeDuration = 1f; // Duration of the fade-out effect
        float elapsed = 0f;

        // Ensure the fade overlay is visible and starts transparent
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            Color overlayColor = fadeOverlay.color;
            overlayColor.a = 0f;
            fadeOverlay.color = overlayColor;

            // Fade in the overlay
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration); // Gradually increase alpha
                fadeOverlay.color = overlayColor;
                yield return null;
            }
        }

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}

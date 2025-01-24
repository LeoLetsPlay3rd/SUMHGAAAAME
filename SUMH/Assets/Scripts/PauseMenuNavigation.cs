using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuNavigation : MonoBehaviour
{
    public Button resumeButton; // Reference to Resume Button
    public Button restartButton; // Reference to Restart Button
    private Button currentSelectedButton; // Currently selected button

    public CanvasGroup resumeCanvasGroup; // CanvasGroup for Resume Button
    public CanvasGroup restartCanvasGroup; // CanvasGroup for Restart Button

    private void Start()
    {
        // Set default selection to Resume Button
        currentSelectedButton = resumeButton;
        UpdateButtonSelection();
    }

    private void Update()
    {
        // Read input from left stick
        Vector2 navigationInput = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;

        if (navigationInput.y > 0.5f) // Navigate Up
        {
            if (currentSelectedButton == restartButton)
            {
                currentSelectedButton = resumeButton;
                UpdateButtonSelection();
            }
        }
        else if (navigationInput.y < -0.5f) // Navigate Down
        {
            if (currentSelectedButton == resumeButton)
            {
                currentSelectedButton = restartButton;
                UpdateButtonSelection();
            }
        }

        // Handle Button Press with X
        if (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false)
        {
            currentSelectedButton.onClick.Invoke();
        }
    }

    private void UpdateButtonSelection()
    {
        // Update button opacities
        if (currentSelectedButton == resumeButton)
        {
            SetButtonOpacity(resumeCanvasGroup, 1f); // Full opacity
            SetButtonOpacity(restartCanvasGroup, 0.5f); // Low opacity
        }
        else if (currentSelectedButton == restartButton)
        {
            SetButtonOpacity(resumeCanvasGroup, 0.5f); // Low opacity
            SetButtonOpacity(restartCanvasGroup, 1f); // Full opacity
        }
    }

    private void SetButtonOpacity(CanvasGroup buttonCanvasGroup, float opacity)
    {
        buttonCanvasGroup.alpha = opacity;
    }
}

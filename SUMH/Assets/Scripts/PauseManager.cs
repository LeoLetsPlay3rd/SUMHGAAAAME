using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenu; // Pause Menu Panel
    public CanvasGroup canvasGroup; // Canvas Group for fade
    public VideoPlayer videoPlayer; // Video Player for the background

    private bool isPaused = false; // Tracks pause state

    void Update()
    {
        // Listen for the pause button (Escape or Controller Options button)
        if (Input.GetKeyDown(KeyCode.Escape) || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            if (isPaused)
                ResumeGame(); // Resume if already paused
            else
                PauseGame(); // Pause if not paused
        }
    }

    public void PauseGame()
    {
        if (isPaused) return; // Prevent double execution

        isPaused = true;
        pauseMenu.SetActive(true); // Activate the menu
        StartCoroutine(FadeInPauseMenu()); // Start fade-in
        Time.timeScale = 0f; // Freeze the game
        videoPlayer?.Play(); // Play the video background (if assigned)
    }

    public void ResumeGame()
    {
        if (!isPaused) return; // Prevent double execution

        isPaused = false;
        StartCoroutine(FadeOutPauseMenu(() =>
        {
            pauseMenu.SetActive(false); // Deactivate after fade-out
        }));
        Time.timeScale = 1f; // Resume the game
        videoPlayer?.Pause(); // Pause the video background (if assigned)
    }

    public void RestartGame()
    {
        Debug.Log("Restarting application...");
        Application.Quit(); // Quit the application
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); // Restart the application (only works in builds)
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        StartCoroutine(DirectTransitionToMainMenu());
    }

    private IEnumerator DirectTransitionToMainMenu()
    {
        // Keep the pause menu active during the transition
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1; // Ensure full opacity of the pause menu
        }

        yield return null; // Wait a single frame to ensure smooth execution

        SceneManager.LoadScene("MainMenu"); // Load the main menu without fading the pause menu
    }

    private IEnumerator FadeInPauseMenu()
    {
        canvasGroup.alpha = 0; // Start fully transparent
        float fadeDuration = 0.5f; // Duration of the fade
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time during pause
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // Gradually increase alpha
            yield return null;
        }

        canvasGroup.alpha = 1; // Fully visible
    }

    private IEnumerator FadeOutPauseMenu(System.Action onComplete)
    {
        float fadeDuration = 0.5f; // Duration of the fade
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time during pause
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration)); // Gradually decrease alpha
            yield return null;
        }

        canvasGroup.alpha = 0; // Fully hidden
        onComplete?.Invoke(); // Call the onComplete action (e.g., deactivating the menu)
    }
}

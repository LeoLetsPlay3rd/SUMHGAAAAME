using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SceneController : MonoBehaviour
{
    public Image fadeOverlay;           // Reference to the fade overlay image
    public float fadeDuration = 1f;     // Duration of fade in/out effects
    private bool transitioningScene = false; // Flag for managing scene transitions
    private bool isFading = false;      // Flag to ensure no overlapping fades

    private void Start()
    {
        // Destroy all objects in DontDestroyOnLoad if MainMenu is loaded
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log("MainMenu loaded. Destroying all objects in DontDestroyOnLoad...");
            DestroyDontDestroyOnLoadObjects();
        }

        // Start scene with fade-in if fadeOverlay is present
        if (fadeOverlay != null)
        {
            fadeOverlay.color = new Color(0f, 0f, 0f, 1f); // Fully opaque
            StartCoroutine(FadeInScene());
        }
    }

    private void Update()
    {
        // Only handle input if the active scene is MainMenu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            // Detect Gamepad X Button (South Button) or Keyboard X Key
            if (!transitioningScene &&
                ((Gamepad.current?.buttonSouth.wasPressedThisFrame == true) ||
                 (Keyboard.current?.xKey.wasPressedThisFrame == true)))
            {
                Debug.Log("X pressed in MainMenu. Transitioning to Intro.");
                BeginSceneTransition("Intro");
            }
        }
    }

    /// <summary>
    /// Begins a scene transition with fade-out and loads the specified scene.
    /// </summary>
    public void BeginSceneTransition(string nextSceneName)
    {
        if (transitioningScene) return;

        transitioningScene = true;
        StartCoroutine(FadeOutSceneAndLoadNext(nextSceneName));
    }

    /// <summary>
    /// Triggers a fade to black without transitioning scenes.
    /// </summary>
    public void FadeToBlack()
    {
        if (!isFading && fadeOverlay != null)
        {
            Debug.Log("SceneController: Triggering fade to black...");
            StartCoroutine(FadeOutToBlack());
        }
    }

    /// <summary>
    /// Coroutine to fade out the screen to black.
    /// </summary>
    private IEnumerator FadeOutToBlack()
    {
        if (fadeOverlay == null)
        {
            Debug.LogWarning("SceneController: Fade overlay not assigned!");
            yield break;
        }

        Debug.Log("SceneController: Starting fade to black...");
        isFading = true;

        float elapsed = 0f;
        fadeOverlay.gameObject.SetActive(true); // Ensure the overlay is active

        Color overlayColor = fadeOverlay.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            overlayColor.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration); // Fade to black
            fadeOverlay.color = overlayColor;
            yield return null;
        }

        fadeOverlay.color = new Color(0f, 0f, 0f, 1f); // Ensure fully opaque
        Debug.Log("SceneController: Fade to black complete.");
        isFading = false;
    }

    /// <summary>
    /// Coroutine to fade out and transition to the next scene.
    /// </summary>
    private IEnumerator FadeOutSceneAndLoadNext(string nextSceneName)
    {
        Debug.Log($"SceneController: Fading out and transitioning to {nextSceneName}...");

        float elapsed = 0f;

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            Color overlayColor = fadeOverlay.color;
            overlayColor.a = 0f; // Start fade from transparent
            fadeOverlay.color = overlayColor;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration); // Fade to black
                fadeOverlay.color = overlayColor;
                yield return null;
            }
        }

        SceneManager.LoadScene(nextSceneName);

        yield return new WaitForSeconds(0.1f);

        if (fadeOverlay != null)
        {
            StartCoroutine(FadeInScene());
        }

        transitioningScene = false;
    }

    /// <summary>
    /// Coroutine to fade in the screen from black.
    /// </summary>
    private IEnumerator FadeInScene()
    {
        Debug.Log("SceneController: Fading in scene...");

        float elapsed = 0f;

        if (fadeOverlay != null)
        {
            Color overlayColor = fadeOverlay.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // Fade from black
                fadeOverlay.color = overlayColor;
                yield return null;
            }

            fadeOverlay.color = new Color(0f, 0f, 0f, 0f); // Ensure fully transparent
            fadeOverlay.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Destroys all objects in DontDestroyOnLoad except for the SceneController itself.
    /// </summary>
    private void DestroyDontDestroyOnLoadObjects()
    {
        GameObject[] rootObjects = SceneManager.GetSceneByName("DontDestroyOnLoad").GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj != gameObject) // Exclude this SceneController object itself
            {
                Debug.Log($"Destroying object: {obj.name}");
                Destroy(obj);
            }
        }
    }
}

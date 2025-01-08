using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro UI
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance for global access

    public bool[] objectInteractions; // Tracks if objects have been interacted with
    public Vector3 playerPosition; // Stores the player's position
    public Quaternion playerRotation; // Stores the player's rotation
    private int totalObjectsInteracted = 0; // Counts total interactions

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // Duration for fade effect

    private Image fadeImage; // Reference to the fade UI image

    [Header("Confirmation UI")]
    private GameObject moveForwardUI; // UI Canvas/GameObject for confirmation
    private TMP_Text moveForwardText; // TextMeshPro text for the confirmation message
    private bool isAwaitingConfirmation = false; // Tracks if the confirmation is active

    private bool isFirstScene = true; // Tracks if the player is in the first scene

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures this object persists across scenes
            Debug.Log("GameManager initialized and set to persist.");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate GameManager detected. Destroying this instance.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LocateSceneElements(); // Locate UI elements like fade image and MoveForwardUI

        // Initialize the objectInteractions array if not set
        if (objectInteractions == null || objectInteractions.Length == 0)
        {
            Debug.LogWarning("Object interactions array not set! Initializing with 5 default slots.");
            objectInteractions = new bool[5]; // Default to 5 interactable objects
        }

        // Check if this is the first scene
        if (SceneManager.GetActiveScene().name == "D_Phase_1")
        {
            isFirstScene = true;
        }
        else
        {
            isFirstScene = false;
        }
    }

    private void LocateSceneElements()
    {
        // Locate the FadeImage in the scene by its tag
        fadeImage = GameObject.FindWithTag("FadeImage")?.GetComponent<Image>();

        if (fadeImage == null)
        {
            Debug.LogWarning("FadeImage not found in the scene. Ensure an Image with the 'FadeImage' tag exists.");
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 1); // Fully black at the start
            StartCoroutine(FadeIn());
        }

        // Locate the MoveForwardUI in the scene by its tag
        moveForwardUI = GameObject.FindWithTag("MoveForwardUI");
        if (moveForwardUI != null)
        {
            moveForwardText = moveForwardUI.GetComponentInChildren<TMP_Text>();
            moveForwardUI.SetActive(false); // Hide it initially
        }
        else
        {
            Debug.LogWarning("MoveForwardUI not found in the scene. Ensure a GameObject with the 'MoveForwardUI' tag exists.");
        }
    }

    public void RegisterObjectInteraction(int objectIndex)
    {
        Debug.Log($"RegisterObjectInteraction called for Index: {objectIndex}");

        // If this object hasn't been interacted with
        if (!objectInteractions[objectIndex])
        {
            objectInteractions[objectIndex] = true; // Mark as interacted
            Debug.Log($"Object {objectIndex} marked as interacted.");
            totalObjectsInteracted++;

            Debug.Log($"Total interactions: {totalObjectsInteracted} / {objectInteractions.Length}");

            // If all objects are interacted with, save position and show MoveForwardUI
            if (totalObjectsInteracted == objectInteractions.Length)
            {
                Debug.Log("All objects interacted with! Saving player position...");
                SavePlayerState(GameObject.FindWithTag("Player")); // Save player position and rotation
                ShowMoveForwardUI(); // Show MoveForwardUI
            }
        }
        else
        {
            Debug.Log($"Object {objectIndex} was already interacted with.");
        }
    }

    public void SavePlayerState(GameObject player)
    {
        playerPosition = player.transform.position;
        playerRotation = player.transform.rotation;
        Debug.Log($"Player state saved. Position: {playerPosition}, Rotation: {playerRotation.eulerAngles}");
    }

    public void RestorePlayerState(GameObject player)
    {
        if (!isFirstScene) // Only restore position and rotation if not in the first scene
        {
            player.transform.position = playerPosition;
            player.transform.rotation = playerRotation;
            Debug.Log($"Player state restored. Position: {playerPosition}, Rotation: {playerRotation.eulerAngles}");
        }
        else
        {
            Debug.Log("Player position not restored because this is the first scene.");
        }
    }

    private void ShowMoveForwardUI()
    {
        if (moveForwardUI != null)
        {
            moveForwardUI.SetActive(true);
            moveForwardText.text = "Do you want to move forward? Press 'O' to confirm."; // Update message as needed
            isAwaitingConfirmation = true;
        }
    }

    private void HideMoveForwardUI()
    {
        if (moveForwardUI != null)
        {
            moveForwardUI.SetActive(false);
            isAwaitingConfirmation = false;
        }
    }

    private void Update()
    {
        // Handle confirmation input
        if (isAwaitingConfirmation && (Input.GetKeyDown(KeyCode.O) || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)))
        {
            Debug.Log("Player confirmed scene transition.");
            HideMoveForwardUI();
            StartCoroutine(TransitionToNextScene(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    // Transition to the next scene with a fade effect
    public IEnumerator TransitionToNextScene(int nextSceneIndex)
    {
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeOut()); // Fade out to black
        }

        SceneManager.LoadScene(nextSceneIndex); // Load next scene

        // Wait for the scene to load before continuing
        yield return new WaitForSeconds(0.1f);

        LocateSceneElements(); // Re-locate UI elements after scene load
    }

    // Fade out to black
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

    // Fade in from black
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
}

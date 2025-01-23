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

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName != "D_Phase_Start" && currentSceneName != "D_Phase_1")
            {
                DontDestroyOnLoad(player);
                Debug.Log("Player marked as DontDestroyOnLoad for subsequent levels.");
            }
        }
        else
        {
            Debug.LogWarning("No player object found in this scene. Make sure a player is placed in the scene.");
        }
    }


    private void ResetObjectInteractions()
    {
        totalObjectsInteracted = 0;

        if (objectInteractions == null || objectInteractions.Length == 0)
        {
            objectInteractions = new bool[5]; // Default to 5 interactable objects
        }

        for (int i = 0; i < objectInteractions.Length; i++)
        {
            objectInteractions[i] = false;
        }

        Debug.Log("Object interactions and totalObjectsInteracted reset for the new scene.");
    }

    private void LocateSceneElements()
    {
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

        if (!objectInteractions[objectIndex])
        {
            objectInteractions[objectIndex] = true; // Mark as interacted
            totalObjectsInteracted++;

            Debug.Log($"Total interactions: {totalObjectsInteracted} / {objectInteractions.Length}");

            if (totalObjectsInteracted == objectInteractions.Length)
            {
                Debug.Log("All objects interacted with! Saving player position...");
                SavePlayerState(GameObject.FindWithTag("Player")); // Save player position and rotation
                ShowMoveForwardUI(); // Show MoveForwardUI
            }
        }
    }

    public void SavePlayerState(GameObject player)
    {
        if (player != null)
        {
            playerPosition = player.transform.position;
            playerRotation = player.transform.rotation;
            Debug.Log($"Player state saved. Position: {playerPosition}, Rotation: {playerRotation.eulerAngles}");
        }
    }

    public void RestorePlayerState()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            if (playerPosition != Vector3.zero || playerRotation != Quaternion.identity)
            {
                player.transform.position = playerPosition;
                player.transform.rotation = playerRotation;
                Debug.Log($"Player state restored. Position: {playerPosition}, Rotation: {playerRotation.eulerAngles}");
            }
            else
            {
                Debug.LogWarning("No saved player state found. Placing player at a default position.");
                player.transform.position = new Vector3(0, 1, 0); // Default position
                player.transform.rotation = Quaternion.identity;
            }
        }
    }

    private void ShowMoveForwardUI()
    {
        if (moveForwardUI != null)
        {
            moveForwardUI.SetActive(true);

            moveForwardText.text = "Press (O) to continue";

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
        if (isAwaitingConfirmation && (Input.GetKeyDown(KeyCode.O) || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)))
        {
            Debug.Log("Player confirmed scene transition.");
            HideMoveForwardUI();
            StartCoroutine(TransitionToNextScene(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    public IEnumerator TransitionToNextScene(int nextSceneIndex)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        if (currentSceneName == "D_Phase_Start")
        {
            // Destroy any remaining player objects in the start scene
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                Debug.Log($"Destroying player object: {player.name}");
                Destroy(player);
            }
        }

        SceneManager.LoadScene(nextSceneIndex);

        yield return null;

        LocateSceneElements();

        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
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
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }
}

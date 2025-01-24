using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro UI
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public bool[] objectInteractions; // Tracks object interactions
    public Vector3 playerPosition; // Player position
    public Quaternion playerRotation; // Player rotation
    private int totalObjectsInteracted = 0; // Interaction count

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // Fade duration
    private Image fadeImage; // Fade image reference

    [Header("Confirmation UI")]
    private GameObject moveForwardUI; // Move forward UI reference
    private TMP_Text moveForwardText; // Move forward message
    private bool isAwaitingConfirmation = false; // Confirmation state

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        LocateSceneElements();
        HandleDontDestroyOnLoadObjectsForPhase1_3();
    }

    /// <summary>
    /// Handles the cleaning of non-essential objects when transitioning to D_Phase_1_3.
    /// </summary>
    private void HandleDontDestroyOnLoadObjectsForPhase1_3()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "D_Phase_1_3")
        {
            Debug.Log("Transitioning to D_Phase_1_3. Cleaning up persistent objects...");
            DestroyAllDontDestroyOnLoadObjects(); // Clean up all persistent objects except GameManager
        }
    }

    private void DestroyAllDontDestroyOnLoadObjects()
    {
        GameObject[] dontDestroyObjects = FindObjectsOfType<GameObject>();

        foreach (var obj in dontDestroyObjects)
        {
            if (obj.scene.name == null && obj != gameObject) // Exclude GameManager itself
            {
                Debug.Log($"Destroying object: {obj.name}");
                Destroy(obj);
            }
        }
    }

    private void ResetObjectInteractions()
    {
        totalObjectsInteracted = 0;

        if (objectInteractions == null || objectInteractions.Length == 0)
        {
            objectInteractions = new bool[5];
        }

        for (int i = 0; i < objectInteractions.Length; i++)
        {
            objectInteractions[i] = false;
        }

        Debug.Log("Object interactions and totalObjectsInteracted reset.");
    }

    private void LocateSceneElements()
    {
        fadeImage = GameObject.FindWithTag("FadeImage")?.GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogWarning("FadeImage not found. Ensure it exists in the scene.");
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
            Debug.Log("FadeImage located and initialized.");
        }

        moveForwardUI = GameObject.FindWithTag("MoveForwardUI");
        if (moveForwardUI != null)
        {
            moveForwardUI.SetActive(false);
            Debug.Log("MoveForwardUI located and hidden at start.");
        }
        else
        {
            Debug.LogWarning("MoveForwardUI not found. Ensure it exists in the scene.");
        }
    }

    public void RegisterObjectInteraction(int objectIndex)
    {
        if (!objectInteractions[objectIndex])
        {
            objectInteractions[objectIndex] = true;
            totalObjectsInteracted++;

            Debug.Log($"Total interactions: {totalObjectsInteracted} / {objectInteractions.Length}");

            if (totalObjectsInteracted == objectInteractions.Length)
            {
                Debug.Log("All objects interacted with! Showing MoveForwardUI.");
                ShowMoveForwardUI();
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
                player.transform.position = new Vector3(0, 1, 0);
                player.transform.rotation = Quaternion.identity;
                Debug.LogWarning("No saved player state found. Placing player at a default position.");
            }
        }
    }

    private void ShowMoveForwardUI()
    {
        if (moveForwardUI != null)
        {
            moveForwardUI.SetActive(true);

            // Find the HoverTextDisplay script and hide interaction text
            HoverTextDisplay hoverTextDisplay = FindObjectOfType<HoverTextDisplay>();
            if (hoverTextDisplay != null)
            {
                hoverTextDisplay.HideInteractionText();
            }

            // Clear any existing text before updating
            if (moveForwardText != null)
            {
                moveForwardText.text = "";

                // Update the text based on the current scene
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName == "D_Phase_3")
                {
                    moveForwardText.text = "Press (O) to continue";
                }
                else
                {
                    moveForwardText.text = "Ready to move forward? Press (O)";
                }
            }

            isAwaitingConfirmation = true;
            Debug.Log($"MoveForwardUI is now visible with text: {moveForwardText?.text}");
        }
        else
        {
            Debug.LogWarning("MoveForwardUI is not assigned or missing in the scene.");
        }
    }

    private void HideMoveForwardUI()
    {
        if (moveForwardUI != null)
        {
            moveForwardUI.SetActive(false);
            isAwaitingConfirmation = false;
            Debug.Log("MoveForwardUI is now hidden.");
        }
        else
        {
            Debug.LogWarning("MoveForwardUI is not assigned!");
        }
    }

    private void Update()
    {
        if (isAwaitingConfirmation && (Input.GetKeyDown(KeyCode.O) || (Gamepad.current?.buttonEast.wasPressedThisFrame == true)))
        {
            Debug.Log("Player confirmed scene transition.");
            HideMoveForwardUI();
            StartCoroutine(TransitionToNextScene(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    public IEnumerator TransitionToNextScene(int nextSceneIndex)
    {
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        ResetObjectInteractions();

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

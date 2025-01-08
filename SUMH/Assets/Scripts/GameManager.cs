using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance for global access

    public bool[] objectInteractions; // Tracks if objects have been interacted with
    public Vector3 playerPosition; // Stores the player's position
    public Quaternion playerRotation; // Stores the player's rotation
    private int totalObjectsInteracted = 0; // Counts total interactions

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures this object persists across scenes
            Debug.Log("GameManager initialized and set to persist.");
        }
        else
        {
            Debug.LogWarning("Duplicate GameManager detected. Destroying this instance.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the objectInteractions array if not set
        if (objectInteractions == null || objectInteractions.Length == 0)
        {
            Debug.LogWarning("Object interactions array not set! Initializing with 5 default slots.");
            objectInteractions = new bool[5]; // Default to 5 interactable objects
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

            // If all objects are interacted with, save position and transition scenes
            if (totalObjectsInteracted == objectInteractions.Length)
            {
                Debug.Log("All objects interacted with! Saving player position and switching scenes...");
                SavePlayerState(GameObject.FindWithTag("Player")); // Save player position and rotation
                SceneManager.LoadScene("Phase2"); // Replace "NextScene" with your actual scene name
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
        player.transform.position = playerPosition;
        player.transform.rotation = playerRotation;
        Debug.Log($"Player state restored. Position: {playerPosition}, Rotation: {playerRotation.eulerAngles}");
    }
}

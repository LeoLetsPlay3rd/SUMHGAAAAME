using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class PersistentPlayer : MonoBehaviour
{
    public static PersistentPlayer Instance;

    // Define default position and rotation for D_Phase_1 3
    public Vector3 defaultPositionForPhase1 = new Vector3(-3.694f, -0.633f, 4.595f);
    public Vector3 defaultRotationForPhase1 = new Vector3(0f, -42.261f, 0f); // Euler angles

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event
            Debug.Log("PersistentPlayer initialized.");
        }
        else
        {
            Debug.LogWarning("Duplicate PersistentPlayer detected. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Reset player state when transitioning to D_Phase_1 3
        if (scene.name == "D_Phase_1 3")
        {
            ResetPlayerState();
            EnablePlayerComponents();
        }
    }

    private void ResetPlayerState()
    {
        Debug.Log("ResetPlayerState() called for D_Phase_1 3");

        // Reset player's position
        transform.position = defaultPositionForPhase1;

        // Override rotation completely
        transform.rotation = Quaternion.identity; // Reset to no rotation
        transform.rotation = Quaternion.Euler(defaultRotationForPhase1); // Apply the desired rotation
    }

    private void EnablePlayerComponents()
    {
        Debug.Log("Enabling player components...");

        // Enable PlayerController
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("PlayerController enabled.");
        }

        // Enable InputSystem actions
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = true;
            Debug.Log("PlayerInput enabled.");
        }

        // Enable any other components as needed
        HoverEffectController hoverEffectController = GetComponent<HoverEffectController>();
        if (hoverEffectController != null)
        {
            hoverEffectController.enabled = true;
            Debug.Log("HoverEffectController enabled.");
        }

        // Example: Enable a Character Controller
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = true;
            Debug.Log("CharacterController enabled.");
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

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

        // Enable all disabled components (example for Rigidbody and Collider)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Enable Rigidbody
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true; // Enable Collider
        }

        Debug.Log($"Player state has been reset. Position: {transform.position}, Rotation: {transform.rotation.eulerAngles}");
    }
}

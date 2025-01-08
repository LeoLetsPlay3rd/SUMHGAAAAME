using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    public static PersistentPlayer Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Debug to confirm the camera is present
            Camera playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                Debug.Log("Player's camera is persisting.");
            }
            else
            {
                Debug.LogError("Player's camera is missing!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

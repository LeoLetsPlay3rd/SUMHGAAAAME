using UnityEngine;

public class PlayerPositionRestorer : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestorePlayerState(gameObject); // Restore position and rotation
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Player state not restored.");
        }
    }
}

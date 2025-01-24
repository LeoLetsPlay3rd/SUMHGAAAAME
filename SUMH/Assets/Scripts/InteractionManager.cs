using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{
    public GameObject player; // Assign the player GameObject in the Inspector
    private int totalInteractions = 0;
    private int interactionThreshold = 5; // Number of objects to interact with

    public void RegisterInteraction()
    {
        totalInteractions++;
        Debug.Log("Total interactions: " + totalInteractions);

        if (totalInteractions >= interactionThreshold)
        {
            if (GameManager.Instance != null)
            {
                // Save the player's position and rotation
                GameManager.Instance.SavePlayerState(player);

                // Trigger the next scene by incrementing the current scene index
                Debug.Log("All interactions completed. Transitioning to the next scene...");
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) // Ensure it doesn't exceed scene count
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.LogWarning("No more scenes available in build settings.");
                }
            }
            else
            {
                Debug.LogWarning("GameManager instance is null. Cannot save player state or transition scenes.");
            }
        }
    }
}

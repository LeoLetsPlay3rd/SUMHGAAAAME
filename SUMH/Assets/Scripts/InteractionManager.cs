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
            }
            else
            {
                Debug.LogWarning("GameManager instance is null. Cannot save player state.");
            }

            Debug.Log("Switching to the next scene...");
            SceneManager.LoadScene("Phase2"); // Replace with your actual scene name
        }
    }
}

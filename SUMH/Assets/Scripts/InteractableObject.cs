using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int objectIndex; // Unique index for this object
    public CircleManager circleManager; // Reference to the CircleManager

    public void OnInteract()
    {
        // Update CircleManager
        if (circleManager != null)
        {
            circleManager.RegisterInteraction();
        }
        else
        {
            Debug.LogError("CircleManager reference is missing!");
        }

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            Debug.Log($"Interacted with: {gameObject.name} (Index: {objectIndex})");
            GameManager.Instance.RegisterObjectInteraction(objectIndex);
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
}

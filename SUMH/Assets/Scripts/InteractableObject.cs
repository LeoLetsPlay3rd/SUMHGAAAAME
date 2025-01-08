using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int objectIndex; // Unique index for this object

    public void OnInteract()
    {
        Debug.Log($"Interacted with: {gameObject.name} (Index: {objectIndex})");

        if (GameManager.Instance != null)
        {
            Debug.Log("Notifying GameManager...");
            GameManager.Instance.RegisterObjectInteraction(objectIndex);
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
}

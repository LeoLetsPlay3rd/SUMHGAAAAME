using MyGame.Interactions;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public void OnInteract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        // Add custom interaction logic here
    }
}

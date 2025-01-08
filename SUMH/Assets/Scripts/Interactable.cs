using UnityEngine;
using TMPro;


public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TextMeshPro text3D; // Reference to the 3D TextMeshPro object for this interactable

    [HideInInspector] public bool hasBeenInteracted = false; // Tracks if the object has been interacted with

    void Start()
    {
        if (text3D != null)
        {
            text3D.gameObject.SetActive(false); // Start with text hidden
            Debug.Log("Text3D initialized and hidden for " + gameObject.name);
        }
        else
        {
            Debug.LogError("Text3D is not assigned for " + gameObject.name);
        }
    }

    public void Hover()
    {
        if (text3D != null && !hasBeenInteracted)
        {
            text3D.gameObject.SetActive(true); // Show the text when hovered
            Debug.Log("Text3D shown for hover on " + gameObject.name);
        }
    }

    public void StopHover()
    {
        if (text3D != null && !hasBeenInteracted)
        {
            text3D.gameObject.SetActive(false); // Hide the text when hover stops
            Debug.Log("Text3D hidden for stop hover on " + gameObject.name);
        }
    }

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (!hasBeenInteracted)
        {
            hasBeenInteracted = true; // Mark as interacted

            if (text3D != null)
            {
                text3D.gameObject.SetActive(true); // Show the text after interaction
                Debug.Log("Text3D remains visible after interaction for " + gameObject.name);
            }
        }
    }

    public void ResetInteraction()
    {
        hasBeenInteracted = false; // Reset interaction state
        if (text3D != null)
        {
            text3D.gameObject.SetActive(false); // Hide the text when reset
        }
        Debug.Log("Interaction reset for " + gameObject.name);
    }
}

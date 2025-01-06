using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TextMeshPro text3D; // Reference to the 3D TextMeshPro object for this interactable

    private bool isReflecting = false; // Tracks whether the text is currently reflecting

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
        if (text3D != null && !isReflecting)
        {
            text3D.gameObject.SetActive(true); // Enable the text when hovered
            Debug.Log("Text3D shown for hover on " + gameObject.name);
        }
    }

    public void StopHover()
    {
        if (text3D != null && !isReflecting)
        {
            text3D.gameObject.SetActive(false); // Hide the text when hover stops
            Debug.Log("Text3D hidden for stop hover on " + gameObject.name);
        }
    }

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (text3D != null)
        {
            // Toggle the text visibility
            bool isActive = text3D.gameObject.activeSelf;
            text3D.gameObject.SetActive(!isActive);

            if (isActive)
            {
                Debug.Log("Text3D hidden after interaction with " + gameObject.name);
            }
            else
            {
                Debug.Log("Text3D shown after interaction with " + gameObject.name);
            }
        }
        else
        {
            Debug.LogError("Text3D is not assigned or missing for " + gameObject.name);
        }
    }

    public void ResetReflection()
    {
        isReflecting = false; // Reset the reflecting state
        Debug.Log("Reflection reset for " + gameObject.name);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HoverEffectController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image baseCircle; // Reference to the Base Circle
    public Image hoverCircle; // Reference to the Hover Circle

    [Header("Raycast Settings")]
    public float hoverDistance = 5.0f; // Maximum distance for detecting interactables
    public LayerMask interactableLayer; // Layer for interactable objects

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Debugging hoverCircle and baseCircle
        if (hoverCircle == null)
        {
            Debug.LogError("Hover Circle is not assigned in the Inspector!");
        }
        else if (hoverCircle.GetComponent<Image>() == null)
        {
            Debug.LogError("Hover Circle is missing an Image component!");
        }

        if (baseCircle == null)
        {
            Debug.LogError("Base Circle is not assigned in the Inspector!");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found! Ensure the camera is tagged as 'MainCamera'.");
        }
    }


    void Update()
    {
        if (hoverCircle == null || hoverCircle.GetComponent<Image>() == null)
        {
            Debug.LogError("Hover Circle or its Image component is missing!");
            return; // Exit the Update method if hoverCircle is not assigned
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is null! Ensure the camera is tagged as 'MainCamera'.");
            return; // Exit the Update method if mainCamera is not assigned
        }

        // Cast a ray from the center of the screen
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // Check if the ray hits an interactable object
        if (Physics.Raycast(ray, out hit, hoverDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Guitar"))
            {
                hoverCircle.GetComponent<Image>().enabled = true;
            }
            else
            {
                hoverCircle.GetComponent<Image>().enabled = false;
            }
        }
        else
        {
            hoverCircle.GetComponent<Image>().enabled = false;
        }
    }



    /// <summary>
    /// Handles interaction with the guitar.
    /// </summary>
    /// <param name="guitar">The guitar GameObject being interacted with.</param>
    void InteractWithGuitar(GameObject guitar)
    {
        Debug.Log("Interacted with the guitar: " + guitar.name);
        // Add custom behavior here (e.g., enable strumming animation, play sound, etc.)
    }
}

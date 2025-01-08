using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class HoverEffectController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI hoverText; // Direct reference to the TextMeshProUGUI object
    public Transform canvasTransform; // Parent canvas for hover text
    public Image hoverCircle; // Hover circle image on the canvas
    public float hoverDistance = 5.0f;
    public LayerMask interactableLayer;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (hoverText == null)
        {
            Debug.LogError("Hover Text is not assigned!");
        }

        if (canvasTransform == null)
        {
            Debug.LogError("Canvas Transform is not assigned!");
        }

        if (hoverCircle == null)
        {
            Debug.LogError("Hover Circle is not assigned!");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found! Ensure it's tagged 'MainCamera'.");
        }

        // Make sure the hover circle and text start as hidden
        if (hoverCircle != null)
        {
            hoverCircle.enabled = false;
        }
        if (hoverText != null)
        {
            hoverText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Cast a ray from the center of the screen
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null && !interactable.hasBeenInteracted)
            {
                // Show hover circle
                if (hoverCircle != null) hoverCircle.enabled = true;

                // Show hover text
                if (hoverText != null)
                {
                    hoverText.gameObject.SetActive(true);
                    hoverText.text = "Press [X] to reflect"; // Replace with your desired text
                }

                // Check for interaction via keyboard or controller
                if (Input.GetKeyDown(KeyCode.X) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
                {
                    interactable.Interact();

                    // Hide hover text and circle after interaction
                    if (hoverText != null) hoverText.gameObject.SetActive(false);
                    if (hoverCircle != null) hoverCircle.enabled = false;
                }
            }
            else
            {
                // Hide hover circle and text if the object has been interacted with
                if (hoverCircle != null) hoverCircle.enabled = false;
                if (hoverText != null) hoverText.gameObject.SetActive(false);
            }
        }
        else
        {
            // Remove hover text and circle if not hovering over an interactable
            if (hoverCircle != null) hoverCircle.enabled = false;
            if (hoverText != null) hoverText.gameObject.SetActive(false);
        }
    }
}

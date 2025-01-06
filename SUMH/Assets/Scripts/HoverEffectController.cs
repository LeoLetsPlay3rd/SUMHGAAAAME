using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Import InputSystem namespace for Gamepad support

public class HoverEffectController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image hoverCircle;
    public float hoverDistance = 5.0f;
    public LayerMask interactableLayer;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (hoverCircle == null)
        {
            Debug.LogError("Hover Circle is not assigned!");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found! Ensure it's tagged 'MainCamera'.");
        }

        hoverCircle.enabled = false;
    }

    void Update()
    {
        // Cast a ray from the center of the screen
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverDistance, interactableLayer))
        {
            hoverCircle.enabled = true;

            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    // Check for interaction via keyboard or controller
                    if (Input.GetKeyDown(KeyCode.X) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
                    {
                        Debug.Log("Interaction triggered with " + hit.collider.gameObject.name);
                        interactable.Interact();
                    }
                }
            }
        }
        else
        {
            hoverCircle.enabled = false;
        }
    }
}

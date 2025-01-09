using UnityEngine;
using TMPro;

public class HoverTextDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text interactionText; // TextMeshPro UI element for interaction text

    [Header("Hover Settings")]
    public float hoverDistance = 5.0f; // Distance for detecting interactable objects
    public LayerMask interactableLayer; // LayerMask for interactable objects

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Ensure interaction text starts disabled
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("InteractionText is not assigned or found in the scene.");
        }
    }


    private void Update()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned or found in the scene.");
            return;
        }

        // Cast a ray from the center of the screen
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null && !interactable.hasBeenInteracted)
            {
                // Enable interaction text
                if (interactionText != null)
                {
                    interactionText.text = "Press (X) to reflect";
                    interactionText.gameObject.SetActive(true);
                }
            }
            else
            {
                // Ensure text is hidden if object has been interacted with
                if (interactionText != null && interactable != null && interactable.hasBeenInteracted)
                {
                    interactionText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // Disable interaction text when no interactable is detected
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }


    public void HideInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

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
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverDistance, interactableLayer))
        {
            hoverCircle.enabled = true;

            if (hit.collider.CompareTag("Interactable"))
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    Debug.Log("X pressed while hovering over " + hit.collider.gameObject.name);
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    if (interactable != null)
                    {
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

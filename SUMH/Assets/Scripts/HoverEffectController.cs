using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro UI
using UnityEngine.InputSystem;

public class HoverEffectController : MonoBehaviour
{
    [Header("UI Elements")]
    private Image hoverCircle; // Hover circle image
    private TMP_Text hoverText; // TextMeshPro UI element for hover text
    private Transform canvasTransform; // Parent canvas for hover UI

    [Header("Hover Settings")]
    public float hoverDistance = 5.0f;
    public LayerMask interactableLayer;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        LocateUIElements(); // Dynamically locate UI elements
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load events
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene load events
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LocateUIElements(); // Re-locate UI elements after scene load
    }

    private void LocateUIElements()
    {
        // Locate UI elements dynamically by their tags
        hoverCircle = GameObject.FindWithTag("HoverCircle")?.GetComponent<Image>();
        hoverText = GameObject.FindWithTag("HoverText")?.GetComponent<TMP_Text>();
        canvasTransform = GameObject.FindWithTag("Canvas")?.transform;

        // Ensure the UI elements are initially disabled
        if (hoverCircle != null)
        {
            hoverCircle.enabled = false;
        }

        if (hoverText != null)
        {
            hoverText.gameObject.SetActive(false);
        }

        // Debug logs to verify references
        if (hoverCircle == null)
        {
            Debug.LogWarning("HoverCircle is not assigned or found in the scene.");
        }

        if (hoverText == null)
        {
            Debug.LogWarning("HoverText is not assigned or found in the scene.");
        }

        if (canvasTransform == null)
        {
            Debug.LogWarning("CanvasTransform is not assigned or found in the scene.");
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
                // Enable hover UI elements
                if (hoverCircle != null)
                {
                    hoverCircle.enabled = true;
                }

                if (hoverText != null)
                {
                    hoverText.text = "Press (X) to reflect"; // Update hover text
                    hoverText.gameObject.SetActive(true);
                }

                // Check for interaction input
                if (Input.GetKeyDown(KeyCode.X) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
                {
                    interactable.Interact();

                    // Disable hover UI after interaction
                    if (hoverCircle != null)
                    {
                        hoverCircle.enabled = false;
                    }

                    if (hoverText != null)
                    {
                        hoverText.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // Disable hover UI when not hovering over an interactable
            if (hoverCircle != null)
            {
                hoverCircle.enabled = false;
            }

            if (hoverText != null)
            {
                hoverText.gameObject.SetActive(false);
            }
        }
    }
}

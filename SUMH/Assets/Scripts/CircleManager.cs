using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CircleManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image[] circles; // Assign your circle UI elements in the Inspector

    [Header("Default Sprites")]
    public Sprite phaseNone; // Default sprite (empty circle)

    [Header("Scene-Specific Sprites")]
    public Sprite phase1Filled; // Filled sprite for Phase 1
    public Sprite phase2Filled; // Filled sprite for Phase 2
    public Sprite phase3Filled; // Filled sprite for Phase 3

    private int currentInteractionCount = 0;

    void Start()
    {
        ResetCircles(); // Ensure circles start with the default sprite
    }

    // Call this method whenever an object is interacted with
    public void RegisterInteraction()
    {
        if (currentInteractionCount < circles.Length)
        {
            // Assign the filled sprite based on the current scene
            circles[currentInteractionCount].sprite = GetFilledSpriteForCurrentScene();
            currentInteractionCount++;
        }
    }

    // Reset circles for a new phase or test
    public void ResetCircles()
    {
        currentInteractionCount = 0;
        foreach (Image circle in circles)
        {
            circle.sprite = phaseNone; // Set all circles to the default sprite
        }
    }

    // Get the correct filled sprite based on the current scene
    private Sprite GetFilledSpriteForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        switch (currentSceneName)
        {
            case "D_Phase_1":
                return phase1Filled;
            case "D_Phase_2":
                return phase2Filled;
            case "D_Phase_3":
                return phase3Filled;
            default:
                Debug.LogWarning("No filled sprite assigned for this scene!");
                return phaseNone; // Default if no match is found
        }
    }
}

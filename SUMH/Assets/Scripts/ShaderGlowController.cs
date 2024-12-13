using UnityEngine;

public class ShaderGlowController : MonoBehaviour
{
    [Header("Material Settings")]
    public Material targetMaterial; // The material with the custom shader
    public string outlineColorProperty = "_OutlineColor"; // Shader property for the outline color
    public string emissionIntensityProperty = "_EmissionIntensity"; // Shader property for emission intensity
    public Color glowColor = Color.yellow; // Default glow color

    [Header("Emission Intensity Settings")]
    public float maxGlowIntensity = 5.0f; // Maximum emission intensity
    public float minGlowIntensity = 2.0f; // Minimum emission intensity
    public float pulseSpeed = 2.0f; // Speed of the pulsating glow

    [Header("Control Settings")]
    public bool isGlowEnabled = true; // Controls whether the glow effect is active

    private float pulseTimer = 0.0f; // Tracks time for pulsating glow

    void Start()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("No target material assigned to ShaderGlowController.");
            enabled = false;
            return;
        }

        ApplyGlowSettings(); // Apply initial glow settings
    }

    void Update()
    {
        if (isGlowEnabled)
        {
            UpdatePulsatingGlow();
        }
    }

    /// <summary>
    /// Updates the pulsating glow effect over time within the defined range.
    /// </summary>
    void UpdatePulsatingGlow()
    {
        // Calculate pulsating intensity within the min and max range
        pulseTimer += Time.deltaTime * pulseSpeed;
        float intensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, (Mathf.Sin(pulseTimer) + 1.0f) / 2.0f);

        // Apply the pulsating glow effect
        SetEmissionIntensity(intensity);
    }

    /// <summary>
    /// Enables or disables the glow effect.
    /// </summary>
    /// <param name="enabled">True to enable glow, false to disable.</param>
    public void SetGlow(bool enabled)
    {
        isGlowEnabled = enabled;

        if (enabled)
        {
            ApplyGlowSettings();
        }
        else
        {
            SetEmissionIntensity(minGlowIntensity); // Set to minimum intensity when disabled
            SetGlowColor(Color.black); // Reset glow to black
        }
    }

    /// <summary>
    /// Applies the current glow settings to the material.
    /// </summary>
    private void ApplyGlowSettings()
    {
        float currentIntensity = GetEmissionIntensity(); // Preserve current intensity
        SetGlowColor(glowColor); // Update the color
        SetEmissionIntensity(Mathf.Clamp(currentIntensity, minGlowIntensity, maxGlowIntensity)); // Restore intensity within bounds
    }

    /// <summary>
    /// Sets the glow color on the material without altering the intensity.
    /// </summary>
    /// <param name="color">Color to apply to the glow.</param>
    private void SetGlowColor(Color color)
    {
        if (targetMaterial.HasProperty(outlineColorProperty))
        {
            targetMaterial.SetColor(outlineColorProperty, color);
        }
        else
        {
            Debug.LogWarning($"Material does not have property {outlineColorProperty}");
        }
    }

    /// <summary>
    /// Sets the emission intensity on the material.
    /// </summary>
    /// <param name="intensity">Intensity value to apply.</param>
    private void SetEmissionIntensity(float intensity)
    {
        if (targetMaterial.HasProperty(emissionIntensityProperty))
        {
            // Clamp the intensity within the defined range
            intensity = Mathf.Clamp(intensity, minGlowIntensity, maxGlowIntensity);
            targetMaterial.SetFloat(emissionIntensityProperty, intensity);
        }
        else
        {
            Debug.LogWarning($"Material does not have property {emissionIntensityProperty}");
        }
    }

    /// <summary>
    /// Gets the current emission intensity from the material.
    /// </summary>
    /// <returns>The current emission intensity.</returns>
    private float GetEmissionIntensity()
    {
        if (targetMaterial.HasProperty(emissionIntensityProperty))
        {
            return targetMaterial.GetFloat(emissionIntensityProperty);
        }

        Debug.LogWarning($"Material does not have property {emissionIntensityProperty}");
        return minGlowIntensity; // Default to min if the property is missing
    }

    /// <summary>
    /// Updates material settings when values are changed in the Inspector.
    /// </summary>
    void OnValidate()
    {
        if (targetMaterial != null && isGlowEnabled)
        {
            ApplyGlowSettings();
        }
    }
}

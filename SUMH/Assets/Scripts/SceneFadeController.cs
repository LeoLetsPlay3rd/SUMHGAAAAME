using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage; // Reference to the fade image
    public float fadeDuration = 1f; // Duration of the fade-in effect

    private void Start()
    {
        // Ensure the fade image starts fully black and then fades in
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1); // Fully black at start
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogError("FadeImage not assigned in SceneFadeController. Please assign it in the Inspector.");
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / fadeDuration); // Reduce alpha over time
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = alpha;
                fadeImage.color = color;
            }
            yield return null;
        }

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f; // Ensure fully transparent at the end
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(false); // Disable the fade image after fade-in
        }
    }
}

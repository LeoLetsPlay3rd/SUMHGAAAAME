using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoCrossfadeManager : MonoBehaviour
{
    public VideoPlayer firstVideoPlayer;  // Reference to the first video
    public VideoPlayer secondVideoPlayer; // Reference to the second video
    public VideoPlayer thirdVideoPlayer;  // Reference to the third video
    public RawImage firstVideoDisplay;   // RawImage for the first video
    public RawImage secondVideoDisplay;  // RawImage for the second video
    public RawImage thirdVideoDisplay;   // RawImage for the third video
    public Image fadeOverlay;            // UI Image for fade-to-black
    public float crossfadeDuration = 1f; // Duration of the crossfade effect
    public float fadeToBlackDuration = 1f; // Duration for fading to black

    private VideoPlayer currentVideoPlayer; // Tracks the currently playing video
    private RawImage currentVideoDisplay;   // Tracks the currently visible video display

    void Start()
    {
        // Sync all videos to the same start time
        SyncAllVideos(0f);

        // Play only the first video
        firstVideoPlayer.Play();
        firstVideoPlayer.time = 0; // Ensure it starts at the beginning
        secondVideoPlayer.Stop();
        thirdVideoPlayer.Stop();

        // Set initial visibility
        firstVideoDisplay.color = new Color(1f, 1f, 1f, 1f); // Fully visible
        secondVideoDisplay.color = new Color(1f, 1f, 1f, 0f); // Fully transparent
        thirdVideoDisplay.color = new Color(1f, 1f, 1f, 0f); // Fully transparent

        // Initialize current video references
        currentVideoPlayer = firstVideoPlayer;
        currentVideoDisplay = firstVideoDisplay;

        // Ensure the fade overlay is transparent
        if (fadeOverlay != null)
        {
            fadeOverlay.color = new Color(0f, 0f, 0f, 0f); // Fully transparent
        }
    }

    public void TransitionToSecondVideo()
    {
        StartCoroutine(CrossfadeToVideo(secondVideoPlayer, secondVideoDisplay));
    }

    public void TransitionToThirdVideo()
    {
        StartCoroutine(CrossfadeToVideo(thirdVideoPlayer, thirdVideoDisplay));
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeOutToBlack());
    }

    private IEnumerator CrossfadeToVideo(VideoPlayer newVideoPlayer, RawImage newVideoDisplay)
    {
        // Step 1: Sync videos
        SyncAllVideos(currentVideoPlayer.time);

        // Step 2: Start the new video
        newVideoPlayer.Play();

        // Step 3: Perform the crossfade
        float elapsed = 0f;
        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / crossfadeDuration;

            // Fade out the current video
            currentVideoDisplay.color = new Color(1f, 1f, 1f, 1f - alpha);

            // Fade in the new video
            newVideoDisplay.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        // Step 4: Finalize the transition
        currentVideoDisplay.color = new Color(1f, 1f, 1f, 0f); // Fully transparent
        newVideoDisplay.color = new Color(1f, 1f, 1f, 1f);     // Fully visible

        // Step 5: Update current video references
        currentVideoPlayer = newVideoPlayer;
        currentVideoDisplay = newVideoDisplay;
    }

    private IEnumerator FadeOutToBlack()
    {
        if (fadeOverlay == null)
        {
            Debug.LogWarning("Fade overlay is not assigned!");
            yield break;
        }

        float elapsed = 0f;

        // Gradually fade the overlay to black
        while (elapsed < fadeToBlackDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / fadeToBlackDuration;
            fadeOverlay.color = new Color(0f, 0f, 0f, alpha); // Increase alpha
            yield return null;
        }

        // Ensure the fade overlay is fully opaque
        fadeOverlay.color = new Color(0f, 0f, 0f, 1f);
    }

    private void SyncAllVideos(double time)
    {
        // Sync all videos to the same playback time
        firstVideoPlayer.time = time;
        secondVideoPlayer.time = time;
        thirdVideoPlayer.time = time;
    }
}
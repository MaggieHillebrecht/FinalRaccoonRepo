using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class SettingAnimationVideo : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public RawImage backgroundImage;

    [Header("Video Clips")]
    public VideoClip forwardVideo;
    public VideoClip reverseVideo;

    [Header("Settings UI")]
    public GameObject settingsUIContainer;

    [Header("Timing")]
    [SerializeField] private float secondsBeforeEndToShowUI = 1f;

    private bool isPlayingForward;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayForward()
    {
        isPlayingForward = true;
        PlayVideo(forwardVideo);
    }

    public void Exit()
    {
        isPlayingForward = false;
        PlayVideo(reverseVideo);
    }

    private void PlayVideo(VideoClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (videoPanel != null)
            videoPanel.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.clip = clip;

        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        videoPlayer.Prepare();
        Debug.Log("Preparing video...");

        while (!videoPlayer.isPrepared)
            yield return null;

        Debug.Log("Video Prepared. Length: " + videoPlayer.length);

        videoPlayer.Play();
        Debug.Log("Video Play called");

        yield return null; // wait one frame so video actually starts

        // Always show video image
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(true);

        // If playing forward, schedule UI reveal
        if (isPlayingForward)
            StartCoroutine(ShowUIBeforeVideoEnds());
    }

    private IEnumerator ShowUIBeforeVideoEnds()
    {
        double videoLength = videoPlayer.length;
        double triggerTime = videoLength - secondsBeforeEndToShowUI;

        if (triggerTime < 0)
            triggerTime = 0;

        Debug.Log("UI will show at time: " + triggerTime);

        // Wait until video reaches the trigger time
        while (videoPlayer.time < triggerTime)
            yield return null;

        Debug.Log("Activating Settings UI Container");

        if (settingsUIContainer != null)
            settingsUIContainer.SetActive(true);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!isPlayingForward)
        {
            if (videoPanel != null)
                videoPanel.SetActive(false);
        }
    }
}
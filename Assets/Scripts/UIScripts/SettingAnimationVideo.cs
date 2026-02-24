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

    private Coroutine backgroundCoroutine;
    private bool isPlayingForward = true;

    public void PlayForward()
    {
        isPlayingForward = true;
        PlayVideo(forwardVideo);
    }

    public void Exit()
    {
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        isPlayingForward = false;
        PlayVideo(reverseVideo);
    }

    private void Start()
    {
        StartCoroutine(PrewarmVideo());
    }

    private IEnumerator PrewarmVideo()
    {
        if (videoPlayer == null || forwardVideo == null)
            yield break;

        videoPlayer.clip = forwardVideo;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.frame = 0;

        videoPlayer.Stop();
    }

    private void PlayVideo(VideoClip clip)
    {
        if (videoPanel != null)
            videoPanel.SetActive(true);

        if (videoPlayer == null || clip == null)
            return;

        // Hide video visually immediately
        if (backgroundImage != null)
            backgroundImage.enabled = false;

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.loopPointReached += OnVideoFinished;

        if (backgroundCoroutine != null)
            StopCoroutine(backgroundCoroutine);

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.frame = 0;
        videoPlayer.Play();

        // Wait for first actual frame render
        StartCoroutine(EnableVideoWhenReady());

        if (isPlayingForward)
            backgroundCoroutine = StartCoroutine(ShowBackgroundWithDelay(5.3f));
    }

    private IEnumerator EnableVideoWhenReady()
    {
        while (videoPlayer.frame <= 0)
            yield return null;

        if (backgroundImage != null)
            backgroundImage.enabled = true;
    }

    private IEnumerator ShowBackgroundWithDelay(float secondsBeforeEnd)
    {
        while (!videoPlayer.isPrepared)
            yield return null;

        double delay = videoPlayer.length - secondsBeforeEnd;

        if (delay < 0)
            delay = 0;

        yield return new WaitForSeconds((float)delay);

        if (isPlayingForward && backgroundImage != null)
            backgroundImage.gameObject.SetActive(true);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (isPlayingForward)
        {
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            if (videoPanel != null)
                videoPanel.SetActive(false);
        }

        vp.loopPointReached -= OnVideoFinished;
    }
}

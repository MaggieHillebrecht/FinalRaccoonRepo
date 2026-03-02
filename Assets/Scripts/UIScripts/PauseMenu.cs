using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] PlayerInputReader input;

    void Start()
    {
        ResetPauseUI();
    }

    void OnEnable()
    {
        if (input != null)
            input.OnPausePressed += TogglePause;
    }

    void OnDisable()
    {
        if (input != null)
            input.OnPausePressed -= TogglePause;
    }

    public void TogglePause()
    {
        if (GameStateController.IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

        pausePanel.SetActive(true);
        GameStateController.Instance.PauseGame();

        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        ResetPauseUI();
        GameStateController.Instance.ResumeGame();

        if (input != null)
            input.enabled = true;
    }

    void ResetPauseUI()
    {
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
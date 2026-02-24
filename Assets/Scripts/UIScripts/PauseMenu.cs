using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject pauseMenuCanvas;

    void Start()
    {
        ResetPauseUI();
    }

    // Called by New Input System
    public void OnPause(InputValue value)
    {
        // Only act on press, ignore release
        if (!value.isPressed) return;

        if (GameStateController.IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        EnsureCanvasActive();

        pausePanel.SetActive(true);
        GameStateController.Instance.PauseGame();

        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        ResetPauseUI();
        GameStateController.Instance.ResumeGame();
    }

    void ResetPauseUI()
    {
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    void EnsureCanvasActive()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);
    }
}
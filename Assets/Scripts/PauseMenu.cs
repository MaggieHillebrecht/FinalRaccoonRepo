using UnityEngine;
using UnityEngine.EventSystems;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateController.IsPaused)
                Resume();
            else
                Pause();
        }
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
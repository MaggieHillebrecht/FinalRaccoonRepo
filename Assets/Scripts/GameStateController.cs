using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance;

    public static bool IsPaused { get; private set; }
    public static bool IsMapOpen { get; private set; }
    public static bool IsGameOver { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ResumeGame();
    }

    public void PauseGame()
    {
        if (IsGameOver) return;

        Time.timeScale = 0f;
        Physics.autoSimulation = false;
        IsPaused = true;

        AkSoundEngine.SetState("PauseState", "Paused");
    }

    public void ResumeGame()
    {
        if (IsGameOver) return;

        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        IsPaused = false;

        AkSoundEngine.SetState("PauseState", "Unpaused");
    }

    public void OpenMap()
    {
        if (IsGameOver) return;

        IsMapOpen = true;
        PauseGame();
    }

    public void CloseMap()
    {
        if (IsGameOver) return;

        IsMapOpen = false;
        ResumeGame();
    }

    public void GameOver()
    {
        IsGameOver = true;

        Time.timeScale = 0f;
        Physics.autoSimulation = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        IsGameOver = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
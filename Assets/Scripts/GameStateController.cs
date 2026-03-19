using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Won,
    GameOver
}

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private Transform playerStartPoint;
    [SerializeField] private GameObject player;
    public static bool forceMainMenu = false; 

    public static GameState CurrentState { get; private set; }

    public static bool gameStarted = false; 

    public static bool IsPaused { get; private set; }
    public static bool IsMapOpen { get; private set; }
    public static bool IsGameOver { get; private set; }
    public static bool IsGameWon { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!gameStarted || forceMainMenu)
        {
            SetState(GameState.MainMenu);
        }
        else
        {
            SetState(GameState.Playing);
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                Physics.autoSimulation = true;
                if (winScreen != null) winScreen.SetActive(false);
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;
                break;

            case GameState.Won:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;
                if (winScreen != null) winScreen.SetActive(true);
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;
                break;
        }
    }

    public void RestartGame()
    {
        forceMainMenu = false; // skip main menu on restart
        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused &&
            CurrentState != GameState.MainMenu) return;

        SetState(GameState.Playing);
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

    public void WinGame()
    {
        if (CurrentState == GameState.Won) return;
        Debug.Log("Game Won!");
        SetState(GameState.Won);
    }
}
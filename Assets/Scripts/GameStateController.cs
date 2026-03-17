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
    [SerializeField] GameObject winScreen;
    public Transform playerStartPoint;
    public GameObject player;
    public static GameState CurrentState { get; private set; }


    public static bool IsPaused { get; private set; }
    public static bool IsMapOpen { get; private set; }
    public static bool IsGameOver { get; private set; }
    public static bool IsGameWon { get; private set; }



    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetState(GameState.MainMenu);
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
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;
                break;

            case GameState.Won:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;

                if (winScreen != null)
                    winScreen.SetActive(true);

                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                Physics.autoSimulation = false;
                break;
        }
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused &&
            CurrentState != GameState.MainMenu)
            return;

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

    public void RestartGame()
    {
        if (winScreen != null)
            winScreen.SetActive(false);

        SetState(GameState.Playing);
    }
}
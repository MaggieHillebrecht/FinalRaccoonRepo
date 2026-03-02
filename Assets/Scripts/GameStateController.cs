using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance;

    public static bool IsPaused { get; private set; }
    public static bool IsMapOpen { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Physics.autoSimulation = false;
        IsPaused = true;

        AkSoundEngine.SetState("PauseState", "Paused");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        IsPaused = false;
        IsMapOpen = false;

        AkSoundEngine.SetState("PauseState", "Unpaused");
    }

    public void OpenMap()
    {
        IsMapOpen = true;
        PauseGame();
    }

    public void CloseMap()
    {
        IsMapOpen = false;
        ResumeGame();
    }
}
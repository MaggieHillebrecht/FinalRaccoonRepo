using UnityEngine;
using UnityEngine.EventSystems;
using AK.Wwise;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject mainMenuCanvas;
    public GameObject startButton;

    [Header("Wwise")]
    public State GameplayState;
    public AK.Wwise.Event musicEvent;

    void Awake()
    {
        // Safely show main menu without null reference
        ShowMainMenuSafe();

        // Safely post music event if it exists
        if (musicEvent != null)
        {
            musicEvent.Post(
                gameObject,
                (uint)AkCallbackType.AK_MusicSyncUserCue,
                MusicCallback);

            Debug.Log("Music Event: " + musicEvent.Name);
        }
        else
        {
            Debug.LogWarning("MusicEvent is not assigned in MainMenuManager.");
        }
    }

    public void ShowMainMenuSafe()
    {
        if (GameStateController.Instance != null)
        {
            if (!GameStateController.forceMainMenu && GameStateController.CurrentState == GameState.Playing)
            {
                // Game is already running, skip showing main menu
                if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
                return;
            }

            GameStateController.Instance.PauseGame();
        }

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);

        if (EventSystem.current != null && startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        }
    }

    public void StartGame()
    {
        GameStateController.forceMainMenu = false; // skip menu next time
        GameStateController.gameStarted = true;    // mark game as started
        if (GameStateController.Instance != null)
            GameStateController.Instance.SetState(GameState.Playing);

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
    }

    private void MusicCallback(object cookie, AkCallbackType type, object info)
    {
        if (type != AkCallbackType.AK_MusicSyncUserCue) return;
        RunGameplayStartLogic();
    }

    private void RunGameplayStartLogic()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);

        if (GameStateController.Instance != null)
            GameStateController.Instance.ResumeGame();
    }
    public void GoToMainMenu()
    {
        GameStateController.forceMainMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    

    public void QuitGame()
    {
        Application.Quit();
    }
}
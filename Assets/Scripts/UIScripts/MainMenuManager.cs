using UnityEngine;
using UnityEngine.EventSystems;
using AK.Wwise;

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
        // Safely enable main menu canvas
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
        else
            Debug.LogWarning("MainMenuCanvas is not assigned.");

        // Safely pause the game
        if (GameStateController.Instance != null)
            GameStateController.Instance.PauseGame();
        else
            Debug.LogWarning("GameStateController.Instance is null.");

        // Safely set UI selection
        if (EventSystem.current != null && startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        }
        else
        {
            Debug.LogWarning("EventSystem or StartButton is not assigned.");
        }
    }

    public void StartGame()
    {
        GameplayState?.SetValue();

        if (GameStateController.Instance != null)
            GameStateController.Instance.SetState(GameState.Playing);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
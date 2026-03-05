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
        ShowMainMenu();

        musicEvent.Post(
            gameObject,
            (uint)AkCallbackType.AK_MusicSyncUserCue,
            MusicCallback);

        Debug.Log("Music Event: " + musicEvent.Name);
    }

    public void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        GameStateController.Instance.PauseGame();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton);
    }
    public void StartGame()
    {
        GameplayState?.SetValue();
    }

    private void MusicCallback(object cookie, AkCallbackType type, object info)
    {
        if (type != AkCallbackType.AK_MusicSyncUserCue) return;

        RunGameplayStartLogic();
    }

    private void RunGameplayStartLogic()
    {
        mainMenuCanvas.SetActive(false);
        GameStateController.Instance.ResumeGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
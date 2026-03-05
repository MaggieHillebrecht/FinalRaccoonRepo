using UnityEngine;
using UnityEngine.InputSystem; // <--- new Input System
using AK.Wwise;

public class TestMusicState : MonoBehaviour
{
    public string stateGroupName = "MusicState"; // Your Wwise State Group
    public string stateName = "Menu";           // The state to switch to

    void Start()
    {
        Debug.Log("TestMusicState is running!");

        if (!AkSoundEngine.IsInitialized())
            Debug.Log("Wwise not initialized yet");
    }

    void Update()
    {
        if (!AkSoundEngine.IsInitialized())
            Debug.Log("Wwise not initialized yet");

        // Check keys using new Input System
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            AkSoundEngine.SetState(stateGroupName, stateName);
            Debug.Log($"Set state {stateName} in group {stateGroupName}");
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            AkSoundEngine.SetState(stateGroupName, "PinkRaccoon");
            Debug.Log("Set state PinkRaccoon");
        }

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            AkSoundEngine.SetState(stateGroupName, "Chase");
            Debug.Log("Set state Chase");
        }
    }
}
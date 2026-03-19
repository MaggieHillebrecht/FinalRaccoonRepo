using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSound : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.oKey.wasPressedThisFrame)
        {
            Debug.Log("INPUT DETECTED (new system)");

            AkSoundEngine.PostEvent("Play_Bark", gameObject);
        }
    }
}
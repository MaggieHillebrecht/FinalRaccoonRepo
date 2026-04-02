using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

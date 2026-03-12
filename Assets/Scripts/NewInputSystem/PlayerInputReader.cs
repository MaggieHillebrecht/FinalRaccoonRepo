using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }

    // Interact triggers once per press
    public bool InteractPressed { get; private set; }

    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnInteractPressed;
    public event Action OnInteractReleased;

    public event Action OnPausePressed;
    public event Action OnMapPressed;

    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
        Debug.Log($"[INPUT] Move received: {Move}");
    }

    public void OnJump(InputValue value)
    {
        JumpHeld = value.isPressed;
        Debug.Log($"[INPUT] Jump: {JumpHeld}");
    }

    public void OnSprint(InputValue value)
    {
        SprintHeld = value.isPressed;
        Debug.Log($"[INPUT] Sprint: {(SprintHeld ? "PRESSED" : "RELEASED")}");
        if (value.isPressed)
            OnSprintPressed?.Invoke();
        else
            OnSprintReleased?.Invoke();
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            InteractPressed = true; // set to true for one frame
            Debug.Log("[INPUT] Interact PRESSED");
            OnInteractPressed?.Invoke();
        }
        else
        {
            Debug.Log("[INPUT] Interact RELEASED");
            OnInteractReleased?.Invoke();
        }
    }

    private void LateUpdate()
    {
        // Reset interact after all scripts have had a chance to read it
        if (InteractPressed)
        {
            Debug.Log("[INPUT] Interact reset at end of frame");
            InteractPressed = false;
        }
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("[INPUT] Pause pressed");
            OnPausePressed?.Invoke();
        }
    }

    public void OnMap(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("[INPUT] Map pressed");
            OnMapPressed?.Invoke();
        }
    }
}
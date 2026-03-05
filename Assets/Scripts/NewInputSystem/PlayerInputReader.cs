using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    // --- Gameplay properties ---
    public Vector2 Move { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool InteractPressed { get; private set; }

    // --- Gameplay events ---
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnInteractPressed;
    public event Action OnInteractReleased;

    // --- UI events ---
    public event Action OnPausePressed;
    public event Action OnMapPressed;

    // --- Gameplay input callbacks ---
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
        InteractPressed = value.isPressed;
        Debug.Log($"[INPUT] Interact: {(InteractPressed ? "PRESSED" : "RELEASED")}");
        if (value.isPressed)
            OnInteractPressed?.Invoke();
        else
            OnInteractReleased?.Invoke();
    }

    // --- UI input callbacks ---
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
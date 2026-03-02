using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction move;
    InputAction jump;
    InputAction sprint;
    InputAction interact;
    InputAction pause;
    InputAction map;

    public Vector2 Move => move.ReadValue<Vector2>();
    public bool JumpHeld => jump.IsPressed();
    public bool SprintHeld => sprint.IsPressed();
    public bool InteractPressed => interact.triggered;
    public event Action OnPausePressed;
    public event Action OnMapPressed;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        move = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];
        sprint = playerInput.actions["Sprint"];
        interact = playerInput.actions["Interact"];
        pause = playerInput.actions["Pause"];
        map = playerInput.actions["Map"];
    }

    void Update()
    {
        if (pause.triggered)
            OnPausePressed?.Invoke();

        if (map.triggered)
            OnMapPressed?.Invoke();
    }
}
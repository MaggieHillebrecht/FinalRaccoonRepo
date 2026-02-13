using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool InteractPressed { get; private set; }

    InputSystem_Actions controls;

    void Awake()
    {
        controls = new InputSystem_Actions();

        controls.Player.Move.performed += c => Move = c.ReadValue<Vector2>();
        controls.Player.Move.canceled += c => Move = Vector2.zero;

        controls.Player.Jump.performed += c => JumpHeld = true;
        controls.Player.Jump.canceled += c => JumpHeld = false;

        controls.Player.Sprint.performed += c => SprintHeld = true;
        controls.Player.Sprint.canceled += c => SprintHeld = false;

        controls.Player.Interact.performed += c => InteractPressed = true;
    }

    void LateUpdate()
    {
        InteractPressed = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}
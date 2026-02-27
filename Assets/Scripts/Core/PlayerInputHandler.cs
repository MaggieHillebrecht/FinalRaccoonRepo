using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveDir { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool InteractHeld { get; private set; }
    public System.Action OnInteractPressed;
    public System.Action OnInteractReleased;
    PlayerJump jump;

    void Awake()
    {
        jump = GetComponent<PlayerJump>();
    }

    public void OnMove(InputValue value)
    {
        Debug.Log("[INPUT] Movement received");
        Vector2 move = value.Get<Vector2>();
        MoveDir = new Vector3(move.x, 0f, move.y);
    }

    public void OnJump()
    {
        Debug.Log("[INPUT] Jump received");
        jump.OnJumpPressed();
    }

    public void OnSprint(InputValue value)
    {
        Debug.Log("[INPUT] Sprint received");
        SprintHeld = value.isPressed;
    }

    public void OnInteract(InputValue value)
    {
        Debug.Log("[INPUT] Interact received");
        if (value.isPressed)
        {
            OnInteractPressed?.Invoke();
        }
        else
        {
            OnInteractReleased?.Invoke();
        }
    }
}

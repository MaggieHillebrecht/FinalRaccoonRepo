using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInputReader input;

    RBMovementMotor motor;
    PlayerSprint sprint;
    PlayerInteractionState state;

    void Awake()
    {
        if (input == null)
            input = GetComponent<PlayerInputReader>();

        motor = GetComponent<RBMovementMotor>();
        sprint = GetComponent<PlayerSprint>();
        state = GetComponent<PlayerInteractionState>();
    }

    void FixedUpdate()
    {
        if (state != null && state.IsMovementBlocked)
        {
            motor.Move(Vector3.zero, 0f);
            return;
        }

        Vector3 moveDir = new Vector3(input.Move.x, 0f, input.Move.y);

        motor.Move(moveDir, sprint.getCurrentSpeed());
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInputReader input; // Updated to use PlayerInputReader
    RBMovementMotor motor;
    PlayerSprint sprint;

    void Awake()
    {
        if (input == null)
            input = GetComponent<PlayerInputReader>();

        motor = GetComponent<RBMovementMotor>();
        sprint = GetComponent<PlayerSprint>();
    }

    void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(input.Move.x, 0f, input.Move.y);

        motor.Move(moveDir, sprint.getCurrentSpeed());

        Debug.Log($"[PlayerController] MoveDir: {moveDir}, SprintMult: {sprint.getCurrentSpeed()}");
    }
}

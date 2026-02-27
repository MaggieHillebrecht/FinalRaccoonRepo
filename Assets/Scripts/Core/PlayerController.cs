using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInputHandler input;
    RBMovementMotor motor;
    PlayerSprint sprint;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        motor = GetComponent<RBMovementMotor>();
        sprint = GetComponent<PlayerSprint>();
    }

    void FixedUpdate()
    {
        motor.Move(input.MoveDir, sprint.CurrentMultiplier);
    }
}

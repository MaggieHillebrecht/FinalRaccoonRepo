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
        // Apply movement with the current sprint multiplier
        // Sprint multiplier is managed by PlayerSprint events, not by SprintHeld
        motor.Move(input.MoveDir, sprint.getCurrentSpeed());
    }
}

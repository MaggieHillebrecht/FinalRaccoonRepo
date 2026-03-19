using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputReader input;

    private PlayerMovement playerMovement;
    private PlayerSprint sprint; 

    void Awake()
    {
        if (input == null) input = GetComponent<PlayerInputReader>();

        playerMovement = GetComponent<PlayerMovement>();
        sprint = GetComponent<PlayerSprint>(); 
    }

    void Update()
    {
        Vector3 moveDir = new Vector3(input.Move.x, 0f, input.Move.y);
        if (moveDir.magnitude > 1f) moveDir.Normalize();

        float sprintMultiplier = sprint != null ? sprint.GetCurrentSpeed() : 1f;

        playerMovement.SetMovementDirection(moveDir, sprintMultiplier); 
    }
}
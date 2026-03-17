using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputReader input;
    private PlayerMovement playerMovement;

    void Awake()
    {
        if (input == null) input = GetComponent<PlayerInputReader>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Vector3 moveDir = new Vector3(input.Move.x, 0f, input.Move.y);
        if (moveDir.magnitude > 1f) moveDir.Normalize();
        playerMovement.SetMovementDirection(moveDir);
    }
}
using UnityEngine;

public class PlayerSprint : MonoBehaviour
{
    public float sprintMultiplier = 1.5f; // reduced from 5, feels more natural

    PlayerInputHandler input;

    public float CurrentMultiplier =>
        input != null && input.SprintHeld ? sprintMultiplier : 1f;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }
}

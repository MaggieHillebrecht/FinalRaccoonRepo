using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerSprint : MonoBehaviour
{
    [SerializeField] float sprintMultiplier = 2f; 
    private PlayerInputReader input;
    private PlayerMovement movement;

    private float CurrentMultiplier { get; set; } = 1f;

    void Awake()
    {
        input = GetComponent<PlayerInputReader>();
        movement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        if (input != null)
        {
            input.OnSprintPressed += Sprint;
            input.OnSprintReleased += Walk;
        }
    }

    void OnDisable()
    {
        if (input != null)
        {
            input.OnSprintPressed -= Sprint;
            input.OnSprintReleased -= Walk;
        }
    }

    void Walk()
    {
        Debug.Log("[PlayerSprint] WALKING");
        UpdateMultiplier(false);
    }

    void Sprint()
    {
        Debug.Log("[PlayerSprint] SPRINT");
        UpdateMultiplier(true);
    }

    void UpdateMultiplier(bool sprinting)
    {
        CurrentMultiplier = sprinting ? sprintMultiplier : 1f;
        Debug.Log($"[PlayerSprint] CurrentMultiplier = {CurrentMultiplier}");

        // Apply multiplier to PlayerMovement
        if (movement != null)
            movement.SetSpeedMultiplier(CurrentMultiplier);
    }

    public float GetCurrentSpeed()
    {
        return CurrentMultiplier;
    }
}
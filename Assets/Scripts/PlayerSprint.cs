using UnityEngine;

public class PlayerSprint : MonoBehaviour
{
    [SerializeField] float sprintMultiplier = 2f; // Can tweak in inspector
    PlayerInputReader input;

    private float CurrentMultiplier { get; set; } = 1f;

    void Awake()
    {
        if (input == null)
            input = GetComponent<PlayerInputReader>();
    }

    void OnEnable()
    {
        input.OnSprintPressed += Sprint;
        input.OnSprintReleased += Walk;
    }

    void OnDisable()
    {
        input.OnSprintPressed -= Sprint;
        input.OnSprintReleased -= Walk;
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
        if (sprinting)
        {
            Debug.Log($"[PlayerSprint] Sprinting - multiplier {sprintMultiplier}");
            CurrentMultiplier = sprintMultiplier;
        }
        else
        {
            CurrentMultiplier = 1f;
        }
    }

    public float getCurrentSpeed()
    {
        return CurrentMultiplier;
    }
}
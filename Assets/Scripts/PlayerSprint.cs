using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    float sprintMultiplier = 2f; // reduced from 5, feels more natural
    PlayerInputHandler input;

    private float CurrentMultiplier { get; set; } = 1f;

    void Awake()
    {
        input = GetComponentInChildren<PlayerInputHandler>();
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
        Debug.Log("WALKING");
        UpdateMultiplier(false);
    }
    void Sprint()
    {
        Debug.Log("SPRINT");
        UpdateMultiplier(true);
    }
    
    void UpdateMultiplier(bool sprinting)
    {
        if (sprinting)
        {
            Debug.Log("[SPRINT] Sprinting - applying multiplier");
            Debug.Log(sprintMultiplier);
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

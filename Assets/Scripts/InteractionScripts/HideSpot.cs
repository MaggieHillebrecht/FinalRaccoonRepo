using UnityEngine;

public class HideSpot : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hidePoint;

    public void Interact(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();

        if (!movement || !state || !sprite)
            return;

        if (!state.IsHidden)
            EnterHide(player, movement, state, sprite);
        else
            ExitHide(movement, state, sprite);
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();

        if (state == null || input == null) return "";

        string key = input.GetInteractKey();

        return state.IsHidden
            ? $"{key} to exit"
            : $"{key} to hide";
    }

    void EnterHide(GameObject player, PlayerMovement movement, PlayerInteractionState state, SpriteRenderer sprite)
    {
        state.EnterHide();

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = Vector3.zero;

        player.transform.position = hidePoint.position;

        movement.enabled = false;
        sprite.enabled = false;
    }

    void ExitHide(PlayerMovement movement, PlayerInteractionState state, SpriteRenderer sprite)
    {
        state.ExitHide();

        movement.enabled = true;
        sprite.enabled = true;
    }
}
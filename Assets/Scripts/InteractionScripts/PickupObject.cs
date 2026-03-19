using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable
{
    Collider objectCollider;
    Collider playerCollider;
    public void Interact(GameObject player)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();
        PlayerInteraction interaction = player.GetComponent<PlayerInteraction>();

        if (!rb || state == null || interaction == null) return;

        if (interaction.currentHeldObject == this)
        {
            Drop(rb, state, interaction);
            return;
        }

        if (interaction.currentHeldObject != null)
            return;

        Pickup(player, rb, state, interaction);
    }

    void Pickup(GameObject player, Rigidbody rb, PlayerInteractionState state, PlayerInteraction interaction)
    {
        Debug.Log("[PICKUP] Picking up");

        rb.isKinematic = true;

        objectCollider = GetComponent<Collider>();
        playerCollider = player.GetComponent<Collider>();

        if (objectCollider && playerCollider)
            Physics.IgnoreCollision(objectCollider, playerCollider, true); 

        Transform holdPoint = interaction.GetHoldPoint();
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        interaction.currentHeldObject = this;
        state.StartHolding();
    }

    void Drop(Rigidbody rb, PlayerInteractionState state, PlayerInteraction interaction)
    {
        Debug.Log("[PICKUP] Dropping");

        rb.isKinematic = false;

        if (objectCollider && playerCollider)
            Physics.IgnoreCollision(objectCollider, playerCollider, false); 

        transform.SetParent(null);

        rb.linearVelocity = interaction.transform.forward * 2f;

        interaction.currentHeldObject = null;
        state.StopHolding();
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        PlayerInteraction interaction = player.GetComponent<PlayerInteraction>();

        if (input == null || interaction == null) return "";

        string key = input.GetInteractKey();

        if (interaction.currentHeldObject == this)
            return $"[{key}] Drop";

        return $"[{key}] Pick Up";
    }
}
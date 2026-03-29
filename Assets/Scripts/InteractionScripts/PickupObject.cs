using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable
{
     private Rigidbody rb;
    private Collider objectCollider;
    private Collider playerCollider;
    private PlayerInteraction interaction;

    public float holdDistance = 1f; 
    public float holdHeight = -.45f;     
    public float followSpeed = 15f;
    private Vector3 lastFacingDir; 

    public void Interact(GameObject player)
    {
        rb = GetComponent<Rigidbody>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();
        PlayerInteraction inter = player.GetComponent<PlayerInteraction>();

        if (!rb || state == null || inter == null) return;

        if (inter.currentHeldObject == this)
        {
            Drop(rb, state, inter);
            return;
        }

        if (inter.currentHeldObject != null) return;

        Pickup(player, rb, state, inter);
    }

    private void Pickup(GameObject player, Rigidbody rb, PlayerInteractionState state, PlayerInteraction inter)
    {
        rb.isKinematic = true;

        objectCollider = GetComponent<Collider>();
        playerCollider = player.GetComponent<Collider>();
        if (objectCollider && playerCollider)
            Physics.IgnoreCollision(objectCollider, playerCollider, true);

        interaction = inter;
        interaction.currentHeldObject = this;
        state.StartHolding();

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null && pm.inputDir.sqrMagnitude > 0.01f)
            lastFacingDir = pm.inputDir.normalized;
        else
            lastFacingDir = player.transform.forward;
    }

    private void Drop(Rigidbody rb, PlayerInteractionState state, PlayerInteraction inter)
    {
        rb.isKinematic = false;

        if (objectCollider && playerCollider)
            Physics.IgnoreCollision(objectCollider, playerCollider, false);

        rb.linearVelocity = inter.transform.forward * 2f;

        interaction.currentHeldObject = null;
        interaction = null;
        state.StopHolding();
    }

    private void LateUpdate()
    {
        if (interaction != null && interaction.currentHeldObject == this)
        {
            PlayerMovement pm = interaction.GetComponent<PlayerMovement>();
            if (pm != null && pm.inputDir.sqrMagnitude > 0.01f)
            {
                lastFacingDir = pm.inputDir.normalized;
            }

            Vector3 targetPos = interaction.transform.position
                                + lastFacingDir * holdDistance
                                + Vector3.up * holdHeight;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(lastFacingDir, Vector3.up);
        }
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
using UnityEngine;
using TMPro;

public class PickupObject : MonoBehaviour, IInteractable
{
    [Header("Pickup Settings")]
    public float holdDistance = 1f;
    public float holdHeight = -0.45f;
    public float followSpeed = 15f;

    [Header("UI & Highlight")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Outline outline;
    [SerializeField] private float highlightRange = 3f;

    private Rigidbody rb;
    private Collider objectCollider;
    private Collider playerCollider;
    private PlayerInteraction interaction;
    private GameObject player;
    private Vector3 lastFacingDir;

    private void Awake()
    {
        if (outline != null) outline.enabled = false;
        if (interactText != null) interactText.gameObject.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool inRange = distance <= highlightRange;

            if (outline != null) outline.enabled = inRange;
            if (interactText != null)
            {
                interactText.gameObject.SetActive(inRange);
                if (inRange) interactText.text = GetInteractText(player);
            }
        }

        if (interaction != null && interaction.currentHeldObject == this)
        {
            PlayerMovement pm = interaction.GetComponent<PlayerMovement>();
            if (pm != null && pm.inputDir.sqrMagnitude > 0.01f)
                lastFacingDir = pm.inputDir.normalized;

            Vector3 targetPos = interaction.transform.position + lastFacingDir * holdDistance + Vector3.up * holdHeight;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(lastFacingDir, Vector3.up);
        }
    }

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
        if (objectCollider != null && playerCollider != null)
            Physics.IgnoreCollision(objectCollider, playerCollider, true);

        interaction = inter;
        interaction.currentHeldObject = this;
        this.player = player;
        state.StartHolding();

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            if (pm.inputDir.sqrMagnitude > 0.01f)
                lastFacingDir = pm.inputDir.normalized; // Use current input direction
            else if (pm.lastMoveDir.sqrMagnitude > 0.01f)
                lastFacingDir = pm.lastMoveDir.normalized; // Use last movement direction
            else
                lastFacingDir = Vector3.forward; // Fallback if standing still
        }
        else
        {
            lastFacingDir = Vector3.forward;
        }

        transform.position = interaction.transform.position + lastFacingDir * holdDistance + Vector3.up * holdHeight;
        transform.rotation = Quaternion.LookRotation(lastFacingDir, Vector3.up);
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

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        PlayerInteraction interaction = player.GetComponent<PlayerInteraction>();
        if (input == null || interaction == null) return "";

        string key = input.GetInteractKey();
        return (interaction.currentHeldObject == this) ? $"[{key}] Drop" : $"[{key}] Pick Up";
    }

    private void OnDrawGizmosSelected()
    {
        if (highlightRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, highlightRange);
        }
    }
}
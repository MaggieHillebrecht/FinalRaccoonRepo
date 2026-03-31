using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Vector3 rangeSize = new Vector3(3f, 2f, 3f);
    public Vector3 rangeOffset = new Vector3(0, 1f, 1f);
    public LayerMask interactLayer;

    [Header("Climb Settings")]
    public float climbSpeed = 3f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("References")]
    [SerializeField] PlayerInputReader input;
    [SerializeField] PlayerMovement movement;
    [SerializeField] Transform holdPoint;

    public PickupObject currentHeldObject;

    // Climbing state (public so other scripts can read them)
    public bool IsClimbing { get; private set; }
    public bool ClimbedFromSide { get; private set; }
    public Vector3 ClimbWallNormal { get; private set; } = Vector3.zero;

    // Internal climb tracking
    private float climbWallTop;
    private float climbStartY;

    Transform PlayerTransform => movement.transform;

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInputReader>();
        if (!movement) movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (input != null)
            input.OnInteractPressed += TryInteract;
    }

    private void OnDisable()
    {
        if (input != null)
            input.OnInteractPressed -= TryInteract;
    }

    private void Update()
    {
        HandleClimbInput();
    }

    // ===== Climb =====

    void HandleClimbInput()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !IsClimbing)
            TryClimb();

        if (IsClimbing && Keyboard.current.spaceKey.wasReleasedThisFrame)
            StopClimb();

        if (!IsClimbing) return;

        if (Keyboard.current.wKey.isPressed)
            PlayerTransform.position += Vector3.up * climbSpeed * Time.deltaTime;
        else if (Keyboard.current.sKey.isPressed)
            PlayerTransform.position -= Vector3.up * climbSpeed * Time.deltaTime;

        bool hasClimbed = PlayerTransform.position.y > climbStartY + 1f;
        if (hasClimbed && PlayerTransform.position.y >= climbWallTop)
        {
            bool wasSideClimb = ClimbedFromSide;
            Vector3 storedNormal = ClimbWallNormal;
            float storedWallTop = climbWallTop;

            StopClimb();

            Vector3 vaultPos = PlayerTransform.position;
            vaultPos.y = storedWallTop + 1f;
            vaultPos += -storedNormal * 1.2f;
            PlayerTransform.position = vaultPos;

            if (wasSideClimb && spriteRenderer != null)
            {
                PlayerTransform.rotation = Quaternion.identity;
                spriteRenderer.flipX = storedNormal.x < 0;
            }
        }
    }

    private void TryClimb()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, 3f))
            {
                if (hit.collider.CompareTag("canClimb"))
                {
                    StartClimb(hit);
                    return;
                }
            }
        }

        Debug.Log("No climbable surface in range");
    }

    private void StartClimb(RaycastHit hit)
    {
        ClimbWallNormal = hit.normal;
        ClimbedFromSide = Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.z);

        PlayerTransform.forward = -hit.normal;
        climbStartY = PlayerTransform.position.y;
        climbWallTop = hit.collider.bounds.max.y;

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        IsClimbing = true;
        if (movement != null)
        {
            movement.enabled = false;
            if (movement.animator != null)
                movement.animator.SetBool("isClimbing", true);
        }

        Debug.Log($"Climb started. Wall top at Y: {climbWallTop}");
    }

    private void StopClimb()
    {
        Rigidbody rb = movement.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = new Vector3(0f, -0.5f, 0f);
            rb.angularVelocity = Vector3.zero;
        }

        IsClimbing = false;

        if (ClimbedFromSide && spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
            PlayerTransform.rotation = Quaternion.identity;
        }

        ClimbedFromSide = false;

        if (movement != null)
        {
            movement.enabled = true;
            if (movement.animator != null)
                movement.animator.SetBool("isClimbing", false);

        }
        Debug.Log("Finished climbing");
    }

    // ===== Interact =====

    void TryInteract()
    {
        // Block interaction while climbing
        if (IsClimbing) return;

        Vector3 center = transform.position + rangeOffset;
        Collider[] hits = Physics.OverlapBox(center, rangeSize / 2f, Quaternion.identity, interactLayer);

        float closest = Mathf.Infinity;
        IInteractable closestInteractable = null;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestInteractable = interactable;
            }
        }

        if (closestInteractable != null)
            closestInteractable.Interact(gameObject);
        else
            Debug.Log("No interactable nearby");
    }

    public Transform GetHoldPoint() => holdPoint;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + rangeOffset, rangeSize);
    }
}
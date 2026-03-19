using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float range = 3f;
    public Transform holdPos;
    public Transform pullPos;
    public bool IsClimbing => isClimbing;
    Transform PlayerTransform => movement.transform;


    [Header("Climb Settings")]
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private float climbWallTop;
    private Vector3 climbWallNormal;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool climbedFromSide;

    private float climbStartY; // ADD THIS

    private Vector2 moveInput;

    [Header("Debug")]
    public bool debugLogs = true;

    [Header("References")]
    [SerializeField] PlayerInputReader input;
    [SerializeField] PlayerMovement movement;

    // Add these public getters
    public bool ClimbedFromSide => climbedFromSide;
    public Vector3 ClimbWallNormal => climbWallNormal;

    // Internal state
    private GameObject heldObj;
    private GameObject pulledObj;
    private ConfigurableJoint joint;

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInputReader>();
        if (!movement) movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (input != null)
            input.OnInteractPressed += HandleInteractPressed;
    }

    private void OnDisable()
    {
        if (input != null)
            input.OnInteractPressed -= HandleInteractPressed;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleInteractPressed()
    {
        if (heldObj || pulledObj)
            Release();
        else
            TryPickupOrPull();
    }

    private void Update()
    {
        if (heldObj)
            heldObj.transform.position = holdPos.position;

        // C starts/stops climbing
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isClimbing && !heldObj && !pulledObj)
        {
            Debug.Log("Trying climb.");
            TryClimb();
        }

        if (isClimbing && Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            StopClimb();
            // PlayerTransform.rotation = Quaternion.identity;
        }

        if (isClimbing)
        {
            if (Keyboard.current.wKey.isPressed)
                PlayerTransform.position += Vector3.up * climbSpeed * Time.deltaTime;
            else if (Keyboard.current.sKey.isPressed)
                PlayerTransform.position -= Vector3.up * climbSpeed * Time.deltaTime;

            bool hasClimbed = PlayerTransform.position.y > climbStartY + 1f;
            if (hasClimbed && PlayerTransform.position.y >= climbWallTop)
            {
                // Store vault data before StopClimb resets climbedFromSide
                bool wasSideClimb = climbedFromSide;
                Vector3 storedNormal = climbWallNormal;
                float storedWallTop = climbWallTop;

                StopClimb(); // re-enables rigidbody and zeros velocity first

                // NOW set position so physics doesn't fight it
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
    }

    private void TryPickupOrPull()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        if (debugLogs) Debug.DrawRay(origin, transform.forward * range, Color.cyan, 1f);

        if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, range))
        {
            if (debugLogs) Debug.Log("Hit: " + hit.collider.name + " | Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("canPickUp"))
                Pickup(hit.collider.attachedRigidbody);
            else if (hit.collider.CompareTag("canPull"))
                Pull(hit.collider.attachedRigidbody);
        }
    }

    private void TryClimb()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, range))
            {
                if (hit.collider.CompareTag("canClimb"))
                {
                    Debug.Log("Object can be climbed.");
                    Climb(hit);
                    return;
                }
            }
        }

        if (debugLogs)
            Debug.Log("No climbable surface in range");
    }

    private void Pickup(Rigidbody rb)
    {
        if (!rb) return;

        heldObj = rb.gameObject;
        rb.isKinematic = true;
        heldObj.transform.SetParent(holdPos);
        heldObj.transform.localPosition = Vector3.zero;

        if (debugLogs)
            Debug.Log($"Picked up {heldObj.name}");
    }

    private void Pull(Rigidbody rb)
    {
        if (!rb) return;

        pulledObj = rb.gameObject;

        joint = pulledObj.AddComponent<ConfigurableJoint>();
        joint.connectedBody = GetComponent<Rigidbody>();

        movement.isPulling = true;

        if (debugLogs)
            Debug.Log($"Started pulling {pulledObj.name}");
    }

    private void Climb(RaycastHit hit)
    {
        climbWallNormal = hit.normal;
    
        // Side approach = normal pointing along X axis
        climbedFromSide = Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.z);
        Rigidbody rb = movement.GetComponent<Rigidbody>();
        PlayerTransform.forward = -hit.normal;
        climbStartY = PlayerTransform.position.y;

        // Rigidbody rb = transform.root.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        isClimbing = true;
        if (movement != null)
            movement.enabled = false;

        // transform.root.forward = -hit.normal;

        climbWallTop = hit.collider.bounds.max.y;
        // climbStartY = transform.root.position.y;

        if (debugLogs)
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

        isClimbing = false;

        // Reset sprite flip if we were climbing from the side
        if (climbedFromSide && spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
            PlayerTransform.rotation = Quaternion.identity;
        }

        climbedFromSide = false;
        if (movement != null)
            movement.enabled = true;

        if (debugLogs)
            Debug.Log("Finished climbing");
    }

    private void Release()
    {
        if (heldObj)
        {
            heldObj.transform.SetParent(null);
            var rb = heldObj.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = false;

            if (debugLogs)
                Debug.Log($"Released {heldObj.name}");

            heldObj = null;
        }

        if (joint)
        {
            Destroy(joint);
            if (debugLogs && pulledObj) Debug.Log($"Stopped pulling {pulledObj.name}");
        }

        if (isClimbing)
            StopClimb();

        pulledObj = null;
        movement.isPulling = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float range = 3f;
    public LayerMask interactLayer;

    [Header("Climb Settings")]
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private float climbWallTop;
    private Vector3 climbWallNormal;
    private float climbStartY;
    private bool climbedFromSide;
    private Quaternion rotationBeforeClimb;
    private float climbCooldown = 0f;
    private float climbCooldownDuration = 1f;

    [Header("References")]
    [SerializeField] PlayerInputReader input;
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] Transform holdPoint;
    [SerializeField] PlayerMovement movement;

    public bool ClimbedFromSide => climbedFromSide;
    public Vector3 ClimbWallNormal => climbWallNormal;
    public bool IsClimbing => isClimbing;
    public PickupObject currentHeldObject;
    private bool lockVertical = false;
    IEnumerator LockVerticalBriefly()
    {
        lockVertical = true;
        yield return new WaitForSeconds(0.3f);
        lockVertical = false;
    }

    void Awake()
    {
        if (!input) input = GetComponent<PlayerInputReader>();
        if (!movement) movement = GetComponent<PlayerMovement>();
        rotationBeforeClimb = movement.transform.rotation;
    }    

    void OnEnable()
    {
        if (input != null)
            input.OnInteractPressed += TryInteract;
    }

    void OnDisable()
    {
        if (input != null)
            input.OnInteractPressed -= TryInteract;
    }

    void Update()
    {
        // Continuously restore rotation while not climbing to prevent getting stuck
        if (!isClimbing)
            movement.transform.rotation = rotationBeforeClimb;

        CheckForInteractable();
        HandleClimbingInput();
    }

    void CheckForInteractable()
    {
        Vector3 center = transform.position + Vector3.up * 1.5f;
        Collider[] hits = Physics.OverlapSphere(center, range);

        IInteractable closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closest = interactable;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            interactText.text = closest.GetInteractText(gameObject);
            interactText.gameObject.SetActive(true);
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, interactLayer);

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

    void HandleClimbingInput()
    {
        if (climbCooldown > 0f)
            climbCooldown -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isClimbing && !currentHeldObject && climbCooldown <= 0f)
            TryClimb();

        if (isClimbing && Keyboard.current.spaceKey.wasReleasedThisFrame)
            StopClimb();

        if (isClimbing)
        {
            // Fallback — if player falls off the wall, stop climbing
            if (movement.transform.position.y < climbStartY - 0.5f)
            {
                StopClimb();
                return;
            }

            if (Keyboard.current.wKey.isPressed && !lockVertical)
                movement.transform.position += Vector3.up * climbSpeed * Time.deltaTime;
            else if (Keyboard.current.sKey.isPressed)
                movement.transform.position -= Vector3.up * climbSpeed * Time.deltaTime;

            bool hasClimbed = movement.transform.position.y > climbStartY + 1f;
            if (hasClimbed && movement.transform.position.y >= climbWallTop)
            {
                Vector3 storedNormal = climbWallNormal;
                float storedWallTop = climbWallTop;

                StopClimb();

                Vector3 vaultPos = movement.transform.position;
                vaultPos.y = storedWallTop + 1f;
                vaultPos += -storedNormal * 1.2f;
                movement.transform.position = vaultPos;

                Rigidbody rb = movement.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                StartCoroutine(LockVerticalBriefly());
            }
        }
    }
    
    void TryClimb()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };
    
        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, range) && hit.collider.CompareTag("canClimb"))
            {
                climbWallNormal = hit.normal;
                climbedFromSide = Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.z);
                rotationBeforeClimb = movement.transform.rotation;

                if (climbedFromSide)
                {
                    Vector3 euler = movement.transform.eulerAngles;
                    euler.y = 90f;
                    movement.transform.eulerAngles = euler;
                }

                climbStartY = movement.transform.position.y;
                climbWallTop = hit.collider.bounds.max.y;

                Rigidbody rb = movement.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                isClimbing = true;
                movement.enabled = false;
                return;
            }
        }

        // No climbable surface found — reset rotation in case player is stuck at 90°
        movement.transform.rotation = rotationBeforeClimb;
        float xScale = rotationBeforeClimb.eulerAngles.y > 90f && rotationBeforeClimb.eulerAngles.y < 270f ? -1f : 1f;
        movement.ResetGraphicsScale(xScale);
    }

    void StopClimb()
    {
        climbCooldown = climbCooldownDuration;

        movement.transform.rotation = rotationBeforeClimb;
        float xScale = rotationBeforeClimb.eulerAngles.y > 90f && rotationBeforeClimb.eulerAngles.y < 270f ? -1f : 1f;
        movement.ResetGraphicsScale(xScale);

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = new Vector3(0f, -0.5f, 0f);
            rb.angularVelocity = Vector3.zero;
        }

        isClimbing = false;
        climbedFromSide = false;
        movement.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.5f, range);
    }
}
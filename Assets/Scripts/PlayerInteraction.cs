using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float range = 3f;
    public Transform holdPos;
    public Transform pullPos;

    [Header("Debug")]
    public bool debugLogs = true;

    [Header("References")]
    [SerializeField] PlayerInputReader input;
    [SerializeField] PlayerMovement movement;

    // Internal state
    private GameObject heldObj;
    private GameObject pulledObj;
    private ConfigurableJoint joint;

    private void Awake()
    {
        // Ensure references are assigned
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

    private void HandleInteractPressed()
    {
        if (heldObj || pulledObj)
        {
            Release();
        }
        else
        {
            TryInteract();
        }
    }

    private void Update()
    {
        // Move held object smoothly
        if (heldObj)
        {
            heldObj.transform.position = holdPos.position;
        }
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 dir = transform.forward;

        if (debugLogs)
            Debug.DrawRay(origin, dir * range, Color.cyan, 1f);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("canPickUp"))
            {
                Pickup(hit.collider.attachedRigidbody);
            }
            else if (hit.collider.CompareTag("canPull"))
            {
                Pull(hit.collider.attachedRigidbody);
            }
        }
        else
        {
            if (debugLogs)
                Debug.Log("No interactable object in range");
        }
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

        // Add a joint to pull the object
        joint = pulledObj.AddComponent<ConfigurableJoint>();
        joint.connectedBody = GetComponent<Rigidbody>();

        movement.isPulling = true;

        if (debugLogs)
            Debug.Log($"Started pulling {pulledObj.name}");
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

        pulledObj = null;
        movement.isPulling = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
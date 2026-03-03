using UnityEngine;

public class ItemInteract : MonoBehaviour
{
   [Header("References")]
    public Rigidbody playerRb;
    public Transform grabPoint;

    [Header("Interaction Settings")]
    public float interactRange = 4f;

    [Header("Pull Settings (Heavy / Hinged Objects)")]
    public float pullForce = 6000f;
    public float pullDamping = 150f;
    public float maxPullSpeed = 4f;

    [Header("Pickup Hold Settings (Light Objects)")]
    public float holdForce = 20000f;
    public float holdDamping = 200f;
    public float maxHoldSpeed = 10f;

    [Header("Layers")]
    public LayerMask pullLayers;
    public LayerMask pickupLayers;

    GameObject grabbedObj;
    Rigidbody grabbedRb;

    PlayerInputHandler input;
    PlayerInteractionState interactionState;

    bool isHoldingPickup = false;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        interactionState = GetComponent<PlayerInteractionState>();
    }

    void OnEnable()
    {
        input.OnInteractPressed += TryGrab;
        input.OnInteractReleased += Drop;
    }

    void OnDisable()
    {
        input.OnInteractPressed -= TryGrab;
        input.OnInteractReleased -= Drop;
    }

    void FixedUpdate()
    {
        if (grabbedRb == null) return;

        if (isHoldingPickup)
        {
            HoldObject();
        }
        else
        {
            PullObject();
        }
    }

    void HoldObject()
    {
        Vector3 targetPos = grabPoint.position;
        Vector3 toTarget = targetPos - grabbedRb.worldCenterOfMass;

        grabbedRb.AddForce(toTarget * holdForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        grabbedRb.AddForce(-grabbedRb.linearVelocity * holdDamping * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (grabbedRb.linearVelocity.magnitude > maxHoldSpeed)
            grabbedRb.linearVelocity = grabbedRb.linearVelocity.normalized * maxHoldSpeed;
    }

    void PullObject()
    {
        Vector3 targetPos = grabPoint.position;
        Vector3 toTarget = targetPos - grabbedRb.worldCenterOfMass;

        toTarget.y = 0f;

        grabbedRb.AddForce(toTarget * pullForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        grabbedRb.AddForce(-grabbedRb.linearVelocity * pullDamping * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (grabbedRb.linearVelocity.magnitude > maxPullSpeed)
            grabbedRb.linearVelocity = grabbedRb.linearVelocity.normalized * maxPullSpeed;
    }

    void TryGrab()
    {
        Debug.Log("TryGrab");
        if (grabbedRb != null) return;

        Collider[] pickupHits = Physics.OverlapSphere(grabPoint.position, interactRange, pickupLayers);
        Debug.Log(pickupHits.Length);
        
        foreach (Collider col in pickupHits)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null || rb.isKinematic) continue;

            grabbedObj = rb.gameObject;
            grabbedRb = rb;

            StartPickup();
            interactionState?.StartPulling();
            return;
        }

        Collider[] pullHits = Physics.OverlapSphere(grabPoint.position, interactRange, pullLayers);
        Debug.Log(pullHits.Length);
        
        foreach (Collider col in pullHits)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null || rb.isKinematic) continue;

            grabbedObj = rb.gameObject;
            grabbedRb = rb;

            isHoldingPickup = false;
            interactionState?.StartPulling();
            return;
        }
        Debug.Log(grabbedObj == null ? "No valid object to grab" : $"Grabbed {grabbedObj.name}");
    }

    void StartPickup()
    {
        Debug.Log("StartPickup");
        isHoldingPickup = true;
        grabbedRb.useGravity = false;
        grabbedRb.linearDamping = 5f;
        grabbedRb.angularDamping = 5f;
    }

    void Drop()
    {
        if (grabbedRb == null) return;

        if (isHoldingPickup)
        {
            grabbedRb.useGravity = true;
            grabbedRb.linearDamping = 0f;
            grabbedRb.angularDamping = 0.05f;
        }

        grabbedObj = null;
        grabbedRb = null;
        isHoldingPickup = false;

        interactionState?.StopPulling();
    }

    void OnDrawGizmosSelected()
    {
        if (grabPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(grabPoint.position, interactRange);
    }
}

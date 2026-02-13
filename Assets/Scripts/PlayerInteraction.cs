using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float range = 3f;
    public Transform holdPos;
    public Transform pullPos;

    [Header("Debug")]
    public bool debugLogs = true;

    PlayerInputReader input;
    PlayerMovement movement;

    GameObject heldObj;
    GameObject pulledObj;
    ConfigurableJoint joint;

    void Awake()
    {
        input = GetComponent<PlayerInputReader>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (input.InteractPressed)
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

        if (heldObj)
            heldObj.transform.position = holdPos.position;
    }

    void TryInteract()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 dir = transform.forward;

        if (debugLogs)
            Debug.DrawRay(origin, dir * range, Color.cyan, 1f);

        RaycastHit hit;

        if (!Physics.Raycast(origin, dir, out hit, range))
        {
            return;
        }

        if (hit.collider.CompareTag("canPickUp"))
        {
            Pickup(hit.collider.attachedRigidbody);
            return;
        }

        if (hit.collider.CompareTag("canPull"))
        {
            Pull(hit.collider.attachedRigidbody);
            return;
        }
    }

    void Pickup(Rigidbody rb)
    {
        if (!rb)
        {
            return;
        }

        heldObj = rb.gameObject;

        rb.isKinematic = true;
        heldObj.transform.SetParent(holdPos);
        heldObj.transform.localPosition = Vector3.zero;
    }

    void Pull(Rigidbody rb)
    {
        if (!rb)
        {
            return;
        }

        pulledObj = rb.gameObject;

        joint = pulledObj.AddComponent<ConfigurableJoint>();
        joint.connectedBody = GetComponent<Rigidbody>();

        movement.isPulling = true;
    }

    void Release()
    {
        if (heldObj)
        {
            heldObj.transform.SetParent(null);
            var rb = heldObj.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = false;

            heldObj = null;
        }

        if (joint)
        {
            Destroy(joint);
        }

        pulledObj = null;
        movement.isPulling = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float range = 3f;
    public LayerMask interactLayer;

    [Header("References")]
    [SerializeField] PlayerInputReader input;
    [SerializeField] TMPro.TextMeshProUGUI interactText;
    [SerializeField] Transform holdPoint; 

    public PickupObject currentHeldObject;
    public bool IsClimbing;
    public bool ClimbedFromSide;
    public Vector3 ClimbWallNormal = Vector3.zero;

    void Awake()
    {
        if (!input)
            input = GetComponent<PlayerInputReader>();
    }

    void Update()
    {
        CheckForInteractable();
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
        {
            closestInteractable.Interact(gameObject);
        }
        else
        {
            Debug.Log("No interactable nearby");
        }
    }

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.5f, range);
    }
}
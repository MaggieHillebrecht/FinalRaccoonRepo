using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Vector3 rangeSize = new Vector3(3f, 2f, 3f); 
    public Vector3 rangeOffset = new Vector3(0, 1f, 1f);
    public LayerMask interactLayer;

    [Header("References")]
    [SerializeField] PlayerInputReader input;
    [SerializeField] TMPro.TextMeshProUGUI interactText;
    [SerializeField] Transform holdPoint;

    public PickupObject currentHeldObject;
    public bool IsClimbing;
    public bool ClimbedFromSide;
    public Vector3 ClimbWallNormal = Vector3.zero;

    private void Awake()
    {
        if (!input)
            input = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        Vector3 center = transform.position + rangeOffset;
        Collider[] hits = Physics.OverlapBox(center, rangeSize / 2f, Quaternion.identity, interactLayer);

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

    void TryInteract()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + rangeOffset, rangeSize);
    }
}
using UnityEngine;
using TMPro;

public class HideSpot : MonoBehaviour, IInteractable
{
    [Header("Hide Settings")]
    [SerializeField] private Transform hidePoint; 
    [SerializeField] private float moveSmooth = 10f; 

    [Header("UI & Highlight")]
    [SerializeField] private TextMeshProUGUI interactText; 
    [SerializeField] private Outline outline;           
    [SerializeField] private float highlightRange = 3f;

    private GameObject player;
    private PlayerMovement movement;
    private PlayerInteractionState state;
    private Rigidbody rb;
    private SpriteRenderer sprite;

    private bool isPlayerHidden => state != null && state.IsHidden;

    private void Awake()
    {
        if (outline != null)
            outline.enabled = false;

        if (interactText != null)
            interactText.gameObject.SetActive(false);

        // Automatically find the player in the scene
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool inRange = distance <= highlightRange;

        // Outline
        if (outline != null)
            outline.enabled = inRange;

        // Interact Text
        if (interactText != null)
        {
            interactText.gameObject.SetActive(inRange);

            // Update text dynamically from player input
            if (inRange)
            {
                interactText.text = GetInteractText(player);
            }
        }

        // Smoothly move player if hidden
        if (isPlayerHidden && hidePoint != null)
        {
            if (rb != null && rb.isKinematic)
            {
                Vector3 targetPos = hidePoint.position;
                player.transform.position = Vector3.Lerp(player.transform.position, targetPos, moveSmooth * Time.deltaTime);
            }
        }
    }

    public void Interact(GameObject interactingPlayer)
    {
        player = interactingPlayer;
        movement = player.GetComponent<PlayerMovement>();
        state = player.GetComponent<PlayerInteractionState>();
        rb = player.GetComponent<Rigidbody>();
        sprite = player.GetComponentInChildren<SpriteRenderer>();

        if (!movement || !state || !sprite)
        {
            Debug.LogWarning("Missing components on player for HideSpot!", this);
            return;
        }

        if (!state.IsHidden)
            EnterHide();
        else
            ExitHide();
    }

    public string GetInteractText(GameObject interactingPlayer)
    {
        PlayerInteractionState s = interactingPlayer.GetComponent<PlayerInteractionState>();
        PlayerInputReader input = interactingPlayer.GetComponent<PlayerInputReader>();
        if (s == null || input == null) return "";

        string key = input.GetInteractKey();
        return s.IsHidden ? $"[{key}] Exit Hide" : $"[{key}] Hide";
    }

    private void EnterHide()
    {
        state.EnterHide();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;        
            rb.detectCollisions = false;  
        }

        if (hidePoint != null)
            player.transform.position = hidePoint.position;

        movement.enabled = false;

        if (sprite != null)
            sprite.enabled = false;
    }

    private void ExitHide()
    {
        state.ExitHide();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        movement.enabled = true;

        if (sprite != null)
            sprite.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (hidePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hidePoint.position, 0.2f);
        }

        if (highlightRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, highlightRange);
        }
    }
}
using UnityEngine;

public class HideSpot : MonoBehaviour, IInteractable
{
    [Header("Hide Settings")]
    [SerializeField] private Transform hidePoint; 
    [SerializeField] private float moveSmooth = 10f; 

    private GameObject player;
    private PlayerMovement movement;
    private PlayerInteractionState state;
    private Rigidbody rb;
    private SpriteRenderer sprite;

    private bool isPlayerHidden => state != null && state.IsHidden;

    private void LateUpdate()
    {
        if (isPlayerHidden && player != null && hidePoint != null)
        {
            Vector3 targetPos = hidePoint.position;
            if (rb != null && rb.isKinematic)
            {
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
            rb.isKinematic = true;        // prevent physics from pushing player
            rb.detectCollisions = false;  
        }

        if (hidePoint != null)
            player.transform.position = hidePoint.position;
        else
            Debug.LogWarning("HidePoint not assigned!", this);

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
    }
}
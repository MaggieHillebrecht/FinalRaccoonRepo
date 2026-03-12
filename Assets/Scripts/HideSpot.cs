using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HideSpot : MonoBehaviour
{
    [Header("Hide Settings")]
    [SerializeField] private Transform hidePoint;
    [SerializeField] private float interactRange = 2f;

    [Header("Player References")]
    [SerializeField] private PlayerInputReader playerInput;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteractionState playerState;
    [SerializeField] private SpriteRenderer playerSprite;

    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.OnInteractPressed += HandleInteract;
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.OnInteractPressed -= HandleInteract;
    }

    private void HandleInteract()
    {
        if (!playerInput || !playerMovement || !playerState || !playerSprite)
            return;

        float distance = Vector3.Distance(playerMovement.transform.position, transform.position);

        if (distance > interactRange)
            return;

        ToggleHide();
    }

    private void ToggleHide()
    {
        if (!playerState.IsHidden)
            EnterHide();
        else
            ExitHide();
    }

    private void EnterHide()
    {
        Debug.Log("[HIDE] Enter Hide");

        playerState.EnterHide();

        Rigidbody rb = playerMovement.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        playerMovement.transform.position = hidePoint.position;

        playerMovement.enabled = false;
        playerSprite.enabled = false;
    }

    private void ExitHide()
    {
        Debug.Log("[HIDE] Exit Hide");

        playerState.ExitHide();

        playerMovement.enabled = true;
        playerSprite.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
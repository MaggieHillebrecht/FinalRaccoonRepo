using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float airControlMultiplier = 0.6f;
    public float stopLerpFactor = 0.15f;

    [Header("Refs")]
    public SpriteRenderer sr;
    public Animator animator;

    private Rigidbody rb;
    public Vector3 inputDir;
    public Vector3 lastMoveDir;
    [SerializeField] private Transform graphics;
    private PlayerInteractionState interactionState;
    private PlayerInteraction playerInteraction; 

    private float currentSpeedMultiplier = 1f;

    void Awake()
    {
        interactionState = GetComponent<PlayerInteractionState>();
        playerInteraction = GetComponent<PlayerInteraction>(); 

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        MovePlayer();
        UpdateAnimation();
        HandleFlip();
    }

    void MovePlayer()
    {
        float control = 1f;

        float pullMultiplier = 1f;
        if (interactionState != null && interactionState.IsPulling)
            pullMultiplier = 0.5f;

        float moveSpeed = speed * currentSpeedMultiplier * pullMultiplier;

        Vector3 targetVel = inputDir * moveSpeed * control;

        Vector3 horiz = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 newHoriz = Vector3.Lerp(horiz, targetVel, stopLerpFactor);

        rb.linearVelocity = new Vector3(newHoriz.x, rb.linearVelocity.y, newHoriz.z);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        // NEW: drive isClimbing bool
        bool climbing = playerInteraction != null && playerInteraction.IsClimbing;
        animator.SetBool("isClimbing", climbing);

        // Skip movement animation logic while climbing
        if (climbing) return;

        Vector3 dir;

        if (interactionState != null && interactionState.IsPulling)
            dir = interactionState.PullDirection;
        else
            dir = inputDir.sqrMagnitude > 0.01f ? inputDir : lastMoveDir;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveZ", dir.z);
        animator.SetBool("isMoving", inputDir.sqrMagnitude > 0.01f);

        animator.speed = currentSpeedMultiplier;
    }

    public void SetMovementDirection(Vector3 dir, float speedMultiplier = 1f)
    {
        inputDir = dir.normalized;

        if (inputDir != Vector3.zero)
            lastMoveDir = inputDir;
    }

    void HandleFlip()
    {
        if (graphics == null) return;

        // Don't flip while climbing
        if (playerInteraction != null && playerInteraction.IsClimbing) return;

        float xDir = 0f;

        if (interactionState != null && interactionState.IsPulling)
            xDir = interactionState.PullDirection.x;
        else
            xDir = inputDir.x;

        if (xDir > 0.01f)
            graphics.localScale = new Vector3(1, 1, 1);
        else if (xDir < -0.01f)
            graphics.localScale = new Vector3(-1, 1, 1);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
    }
}
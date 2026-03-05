using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float airControlMultiplier = 0.6f;
    public float stopLerpFactor = 0.15f;

    [Header("Sprint")]
    public float sprintMultiplier = 1.6f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public float coyoteTime = 0.15f;
    public float jumpCooldown = 0.2f;

    [Header("Pull")]
    public bool isPulling = false;
    public float pullSpeed = 2f;
    public Transform pullPos;

    [Header("Step Climb")]
    public float maxStepHeight = 0.4f;
    public float stepCheckDistance = 0.3f;

    [Header("Refs")]
    public SpriteRenderer sr;
    public Animator animator;
    public GroundChecker groundChecker;
    public LayerMask trampolineLayer;

    Rigidbody rb;
    PlayerInputReader input;

    Vector3 inputDir;
    bool isSprinting;
    bool jumpQueued;

    float lastGroundedTime;
    float lastJumpTime;

    public bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputReader>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimations();
        HandleFlip();
    }

    void FixedUpdate()
    {
        MovePlayer();
        HandleJump();
        HandleStepClimbing();
    }

    void HandleInput()
    {
        Vector2 move = input.Move;
        inputDir = new Vector3(move.x, 0, move.y).normalized;

        isSprinting = input.SprintHeld && inputDir.magnitude > 0.1f && !isPulling;
        jumpQueued = input.JumpHeld;
    }

    void MovePlayer()
    {
        if (!canMove) return;

        float control = groundChecker.IsGrounded ? 1f : airControlMultiplier;
        float currentSpeed = isPulling ? pullSpeed : (isSprinting ? speed * sprintMultiplier : speed);

        Vector3 targetVel = inputDir * currentSpeed * control;

        Vector3 horiz = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 newHoriz = Vector3.Lerp(horiz, targetVel, stopLerpFactor);

        rb.linearVelocity = new Vector3(newHoriz.x, rb.linearVelocity.y, newHoriz.z);
    }

    void HandleJump()
    {
        if (groundChecker.IsGrounded)
            lastGroundedTime = Time.time;

        bool canJump =
            Time.time - lastGroundedTime <= coyoteTime &&
            Time.time - lastJumpTime >= jumpCooldown;

        if (jumpQueued && canJump)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0;
            rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            lastJumpTime = Time.time;
        }
    }

    void HandleAnimations()
    {
        if (!animator) return;
        animator.SetFloat("xVelocity", Mathf.Abs(inputDir.x));
        animator.SetFloat("zVelocity", Mathf.Abs(inputDir.z));
    }

    void HandleFlip()
    {
        if (inputDir.x == 0) return;
        Vector3 s = transform.localScale;
        s.x = Mathf.Sign(inputDir.x) * Mathf.Abs(s.x);
        transform.localScale = s;
    }

    void HandleStepClimbing()
    {
        if (!groundChecker.IsGrounded || inputDir.magnitude == 0) return;

        Vector3 moveDir = inputDir.normalized;

        Vector3 bottom = transform.position + Vector3.up * 0.05f;
        Vector3 top = bottom + Vector3.up * 0.05f;

        if (Physics.CapsuleCast(bottom, top, 0.3f, moveDir, stepCheckDistance, groundChecker.groundMask))
        {
            Vector3 stepBottom = bottom + Vector3.up * maxStepHeight;
            Vector3 stepTop = top + Vector3.up * maxStepHeight;

            if (!Physics.CapsuleCast(stepBottom, stepTop, 0.3f, moveDir, stepCheckDistance, groundChecker.groundMask))
            {
                rb.position += Vector3.up * maxStepHeight;
            }
        }
    }

    public Vector3 GetMovementDirection() => inputDir;
}
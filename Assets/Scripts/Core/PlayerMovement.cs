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
    [SerializeField] private Transform graphics;
    private bool lockFlip = false;

    private Rigidbody rb;
    private Vector3 inputDir;
    private float currentSpeedMultiplier = 1f;

    void Awake()
    {
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
        float moveSpeed = speed * currentSpeedMultiplier;
        Vector3 targetVel = inputDir * moveSpeed;

        Vector3 horiz = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 newHoriz = Vector3.Lerp(horiz, targetVel, stopLerpFactor);
        rb.linearVelocity = new Vector3(newHoriz.x, rb.linearVelocity.y, newHoriz.z);
    }

    void UpdateAnimation()
    {
        if (!animator) return;
        animator.SetFloat("MoveX", inputDir.x);
        animator.SetFloat("MoveZ", inputDir.z);
        animator.speed = currentSpeedMultiplier;
    }

    public void SetMovementDirection(Vector3 dir, float speedMultiplier = 1f)
    {
        inputDir = dir.normalized;
        currentSpeedMultiplier = speedMultiplier;
    }
    
    public void ResetGraphicsScale(float xDir)
    {
        if (graphics == null) return;
        float dir = xDir > 0 ? 1f : -1f;
        graphics.localScale = new Vector3(dir, 1f, 1f);
        lockFlip = false; // explicitly unlock so HandleFlip resumes
    }
    public void SetFacingDirection(float xDir)
    {
        if (graphics == null) return;
        float dir = xDir > 0 ? 1f : -1f;
        graphics.localScale = new Vector3(dir, 1f, 1f);
        lockFlip = true;
    }
    public void UnlockFlip()
    {
        lockFlip = false;
    }
    void HandleFlip()
    {
        if (graphics == null || lockFlip) return;

        if (inputDir.x > 0.01f)
            graphics.localScale = new Vector3(1, 1, 1);
        else if (inputDir.x < -0.01f)
            graphics.localScale = new Vector3(-1, 1, 1);
    }
}
using UnityEngine;

public class RBMovementMotor : MonoBehaviour
{
    public float speed = 10f;
    [Range(0f, 1f)]
    public float smoothing = 0.3f;
    public float airControlMultiplier = 0.6f;
    public float deceleration = 20f;

    Rigidbody rb;
    GroundChecker groundChecker;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!groundChecker)
            groundChecker = GetComponentInChildren<GroundChecker>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void Move(Vector3 input, float speedMultiplier)
    {
        bool grounded = groundChecker != null && groundChecker.IsGrounded;
        float control = grounded ? 1f : airControlMultiplier;

        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (input == Vector3.zero)
        {
            Vector3 brake = -horizontalVelocity * deceleration * Time.fixedDeltaTime;
            rb.AddForce(brake, ForceMode.VelocityChange);
            return;
        }

        Vector3 desiredVelocity = input * speed * speedMultiplier * control;

        if (grounded && groundChecker.GroundNormal != Vector3.zero)
            desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, groundChecker.GroundNormal);

        Vector3 velocityChange = desiredVelocity - horizontalVelocity;
        velocityChange = Vector3.ClampMagnitude(velocityChange, speed);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void SetVerticalVelocity(float yVel)
    {
        Vector3 v = rb.linearVelocity;

        Debug.Log($"[JUMP] BEFORE | y={v.y}");

        v.y = yVel;
        rb.linearVelocity = v;

        Debug.Log($"[JUMP] AFTER | y={rb.linearVelocity.y}");
    }

    public void AddVerticalForce(float force)
    {
        rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
    }
}

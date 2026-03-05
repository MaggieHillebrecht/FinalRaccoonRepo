using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public LayerMask groundMask;
    public float checkRadius = 0.08f;
    public float checkOffset = 10f;
    public float coyoteTime = 0.15f; // grace period after leaving ground

    float lastGroundedTime;

    public bool IsGrounded => Time.time - lastGroundedTime <= coyoteTime;
    public Vector3 GroundNormal { get; private set; } = Vector3.up;

    void FixedUpdate()
    {
        Vector3 checkPos = transform.position + Vector3.down * checkOffset;

        bool hit = Physics.CheckSphere(
            checkPos,
            checkRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (hit)
        {
            lastGroundedTime = Time.time;
            GroundNormal = Vector3.up;
        }
    }
}

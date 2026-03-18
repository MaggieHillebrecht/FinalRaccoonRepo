using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SlipperyPuddle : MonoBehaviour
{
    [Header("Slippery Settings")]
    public Vector3 slideDirection = new Vector3(0, 0, 1); // set this in Inspector to match hallway
    public float slideForce = 20f;  // stronger force to push up ramp
    public float maxSpeed = 12f;    // prevents overshooting
    public PhysicsMaterial slipperyMaterial; // low-friction material

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.rigidbody;
        PlayerInteractionState state = collision.gameObject.GetComponent<PlayerInteractionState>();

        if (state != null)
            state.BlockMovement();  // block movement while on puddle

        // optional: assign low-friction material for smooth sliding
        if (slipperyMaterial != null)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.material = slipperyMaterial;
        }

        // Normalize slide direction in world space
        Vector3 worldDir = transform.TransformDirection(slideDirection.normalized);

        // initial push
        rb.AddForce(worldDir * slideForce, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        PlayerInteractionState state = collision.gameObject.GetComponent<PlayerInteractionState>();
        if (state != null && state.IsHidden) return; // don't slide if hidden

        // Assume grounded if we have any upward-facing contact normal
        bool grounded = false;
        Vector3 averagedNormal = Vector3.zero;

        foreach (var contact in collision.contacts)
        {
            averagedNormal += contact.normal;
            if (contact.normal.y > 0.3f) grounded = true;
        }
        averagedNormal.Normalize();

        if (grounded)
        {
            // Base world direction
            Vector3 worldDir = transform.TransformDirection(slideDirection.normalized);
            worldDir = Vector3.ProjectOnPlane(worldDir, averagedNormal).normalized;
            rb.AddForce(worldDir * slideForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // Clamp velocity while grounded
            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerInteractionState state = collision.gameObject.GetComponent<PlayerInteractionState>();
        if (state != null)
            state.UnblockMovement(); // restore movement

        // reset friction
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.material = null;
    }
}
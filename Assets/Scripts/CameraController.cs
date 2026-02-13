using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -4);
    public float smoothSpeed = 10f;
    public float collisionBuffer = 0.2f;

    [Header("Collision")]
    public LayerMask cameraCollisionMask;

    [Header("Debug")]
    public bool debugLogs = true;
    public bool debugRays = true;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 rayOrigin = player.position + Vector3.up * 1.5f;
        Vector3 desiredPosition = player.position + player.TransformDirection(offset);

        Vector3 direction = desiredPosition - rayOrigin;
        float distance = direction.magnitude;

        Vector3 finalPosition = desiredPosition;

        if (debugRays)
            Debug.DrawRay(rayOrigin, direction, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, direction.normalized, out hit, distance, cameraCollisionMask))
        {
            finalPosition = hit.point - direction.normalized * collisionBuffer;

            if (debugLogs)
                Debug.Log($"[Camera] Hit: {hit.collider.name} at {hit.distance:F2}");
        }
        else
        {
            if (debugLogs)
                Debug.Log("[Camera] No obstruction");
        }

        transform.position = Vector3.Lerp(
            transform.position,
            finalPosition,
            Time.deltaTime * smoothSpeed
        );

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}

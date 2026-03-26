using System;
using UnityEngine;

public class NPCDetection : MonoBehaviour
{
    public float detectionRange = 6f;
    [Range(0f, 360f)] public float viewAngle = 100f;
    
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    public bool CanSeePlayer()
    {
        if (player == null)
            return false;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"{name}: distance to player = {distanceToPlayer}, range = {detectionRange}");
        
        
        if (distanceToPlayer <= detectionRange)
        {
            Debug.Log($"{player.name} is detected by {name} at distance {distanceToPlayer}");
            return true;
        }
        
        return false;
        // Vector3 toPlayer = player.position - transform.position;
        // float distance = toPlayer.magnitude;
        //
        // if (distance > detectionRange)
        //     return false;
        //
        // Vector3 forward = transform.right;
        // float angleToPlayer = Vector3.Angle(forward, toPlayer.normalized);
        //
        // return angleToPlayer <= viewAngle * 0.5f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Vector3 forward = transform.right;
        float halfView = viewAngle * 0.5f;
        
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfView) * forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfView) * forward * detectionRange;

        Gizmos.color = Color.purple;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
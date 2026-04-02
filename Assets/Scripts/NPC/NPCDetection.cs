using System;
using UnityEngine;
using UnityEngine.AI;

public class NPCDetection : MonoBehaviour
{
    public NPCData data;
    public LayerMask obstacleMask;
    
    private Transform player;
    private NavMeshAgent agent;
    private Vector3 facingDirection = Vector3.right;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent != null && agent.velocity.sqrMagnitude > 0.01f)
        {
            facingDirection = agent.velocity.normalized;
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        // If the player has an interaction state and is currently hidden, we shouldn't be able to see them
        var playerState = player.GetComponent<PlayerInteractionState>();
        if (playerState != null && playerState.IsHidden)
        {
            Debug.Log($"{name} cannot see player: player is hidden (IsHidden=true)");
            return false;
        }
        
        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > data.detectionRange)
            return false;
        
        float angleToPlayer = Vector3.Angle(facingDirection, toPlayer.normalized);
        if (angleToPlayer > data.viewAngle * 0.5f)
            return false;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = (player.position + Vector3.up * 0.5f) - rayOrigin;

        if (Physics.Raycast(rayOrigin, rayDirection.normalized, out RaycastHit hit, distanceToPlayer, obstacleMask))
        {
            Debug.Log($"{name} vision blocked by {hit.collider.name}");
            Debug.DrawRay(rayOrigin, rayDirection.normalized * distanceToPlayer, Color.red);
            return false;
        }
        
        return true;
    }

    private void OnDrawGizmos()
    {
        Vector3 forward = facingDirection;
        float halfView = data.viewAngle * 0.5f;
        int segments = 20;

        Gizmos.color = Color.magenta;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-halfView, halfView, i / (float)segments);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;
            Gizmos.DrawLine(transform.position, transform.position + direction * data.detectionRange);
        }
        
        Debug.DrawRay(transform.position, facingDirection * data.detectionRange, Color.yellow);
    }
}
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    public NPCData data;
    private NavMeshAgent agent;

    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    [Header("Patrol Settings")]
    public float reachThreshold = 0.1f; // distance to consider "arrived"

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // keep 2D/top-down vibe
        agent.updatePosition = true;   
        agent.updateUpAxis = false;
    }
    
    public void MoveToPoint(Vector3 target, float speed)
    {
        agent.speed = speed;
        agent.isStopped = false;

        if (!agent.hasPath || Vector3.Distance(agent.destination, target) > 0.1f)
        {
            agent.SetDestination(target);
        }
        
        Debug.Log($"{name} moving to {target}, remainingDistance={agent.remainingDistance}, hasPath={agent.hasPath}, pathPending={agent.pathPending}");

    }
    
    // Public patrol method (no parameter)
    public void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;
        
        Transform patrolPoint = patrolPoints[patrolIndex];
        if (patrolPoint == null)
            return;

        agent.speed = data.moveSpeed;
        agent.isStopped = false;
        agent.SetDestination(patrolPoint.position);
        
        if (!agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;  
        }
    }

    public void Chase(float speed)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        agent.speed = speed;
        agent.isStopped = false;// let NavMeshAgent handle movement
        agent.SetDestination(player.transform.position);
    }

    public bool IsCloseToTarget(Vector3 target, float threshold = 0.5f)
    {
        Vector3 currentPos = transform.position;

        currentPos.y = 0f;
        target.y = 0f;

        return Vector3.Distance(currentPos, target) <= threshold;
    }
    
    public void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    // ===== Debug Gizmos =====
    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            // Draw a sphere at the patrol point
            Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

            // Draw line to next patrol point
            Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];
            if (nextPoint != null)
                Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
        }

        // Optional: draw line to current target
        if (Application.isPlaying && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, patrolPoints[patrolIndex].position);
        }
    }
}
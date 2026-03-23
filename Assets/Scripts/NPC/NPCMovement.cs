using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;      // speed along the lines
    public float reachThreshold = 0.1f; // distance to consider "arrived"

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // keep 2D/top-down vibe
        agent.updatePosition = true;   
        agent.updateUpAxis = false;
    }
    public bool CanReach(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();

        if (agent.CalculatePath(target, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }

        return false;
    }
    public void MoveToPoint(Vector3 target, float speed)
    {
        agent.speed = speed;
        agent.isStopped = false;

        if (!agent.hasPath || Vector3.Distance(agent.destination, target) > 0.1f)
        {
            agent.SetDestination(target);
        }
    }
    
    // Public patrol method (no parameter)
    public void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;
        
        Transform targetPoint = patrolPoints[patrolIndex];
        if (targetPoint == null)
            return;

        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.SetDestination(targetPoint.position);

        Debug.Log($"{name} patrolling to: {targetPoint.position}, remaining: {agent.remainingDistance}");
        
        if (!agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;  
        }
        //
        // agent.updatePosition = false;
        // agent.ResetPath();
        //
        // Transform targetPoint = patrolPoints[patrolIndex];
        // if (targetPoint == null) return;
        //
        // Vector3 direction = targetPoint.position - transform.position;
        //
        // // Arrived at the patrol point
        // if (direction.magnitude <= reachThreshold)
        // {
        //     patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        // }
        // else
        // {
        //     // Move manually along the line
        //     transform.position += direction.normalized * patrolSpeed * Time.deltaTime;
        // }
    }

    public void Chase(float speed)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        agent.speed = speed;
        agent.isStopped = false;// let NavMeshAgent handle movement
        agent.SetDestination(player.transform.position);
    }

    public void Investigate(Vector3 target, float speed)
    {
        agent.speed = speed;
        agent.isStopped = false;
        agent.SetDestination(target);
    }

    public bool ReachedDestination(float threshold = 0.2f)
    {
        if (agent.pathPending)
            return false;

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return true;

        if (!agent.hasPath)
            return true;

        return agent.remainingDistance <= threshold;
    }

    public bool TryGetNearestNavMeshPoint(Vector3 target, float maxDistance, out Vector3 validPoint)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, maxDistance, NavMesh.AllAreas))
        {
            validPoint = hit.position;
            return true;
        }

        validPoint = Vector3.zero;
        return false;
    }
    
    public void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void StopAgent()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
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
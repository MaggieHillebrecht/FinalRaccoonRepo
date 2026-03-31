using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    public NPCData data;
    private NavMeshAgent agent;

    public Transform[] patrolPoints;
    private int patrolIndex;

    [Header("Patrol Settings")]
    public float reachThreshold = 0.1f;

    [Header("Animation")]
    public Animator animator;
    [SerializeField] private Transform graphics;

    private Vector3 lastMoveDir = Vector3.forward;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.updateUpAxis = false;

        if (!animator) animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateAnimation();
        HandleFlip();
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        Vector3 vel = agent.velocity;
        vel.y = 0f;

        bool isMoving = vel.sqrMagnitude > 0.01f;

        Vector3 dir = isMoving ? vel.normalized : lastMoveDir;

        if (isMoving)
            lastMoveDir = dir;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveZ", dir.z);
        animator.SetBool("isMoving", isMoving);
    }

    void HandleFlip()
    {
        if (graphics == null) return;

        float xDir = agent.velocity.x;

        if (xDir > 0.01f)
            graphics.localScale = new Vector3(1, 1, 1);
        else if (xDir < -0.01f)
            graphics.localScale = new Vector3(-1, 1, 1);
    }

    // ===== Everything below is unchanged =====

    public void MoveToPoint(Vector3 target, float speed)
    {
        agent.speed = speed;
        agent.isStopped = false;

        if (!agent.hasPath || Vector3.Distance(agent.destination, target) > 0.1f)
            agent.SetDestination(target);
    }

    public Transform GetCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return null;

        return patrolPoints[patrolIndex];
    }

    public void AdvancePatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    public void Patrol()
    {
        Transform patrolPoint = GetCurrentPatrolPoint();
        if (patrolPoint == null) return;

        MoveToPoint(patrolPoint.position, data.moveSpeed);
    }

    public void Chase(float speed)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        agent.speed = speed;
        agent.isStopped = false;
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

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

            Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];
            if (nextPoint != null)
                Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
        }

        if (Application.isPlaying && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, patrolPoints[patrolIndex].position);
        }
    }
}
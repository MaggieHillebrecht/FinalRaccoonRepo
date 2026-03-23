using UnityEngine;

public class NPCController : MonoBehaviour
{
    public NPCData data;

    public enum NPCState
    {
        Patrol,
        Investigate,
        Chase
    }

    public NPCState currentState = NPCState.Patrol;

    private NPCMovement movement;
    private NPCDetection detection;
    private Transform player;

    private Vector3 investigateTarget;
    private Vector3 lastKnownPlayerPosition;
    private float investigateWaitTimer;
    private float lostSightTimer;

    void Awake()
    {
        movement = GetComponent<NPCMovement>();
        detection = GetComponent<NPCDetection>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (movement == null || detection == null || data == null)
            return;

        bool seesPlayer = detection.CanSeePlayer();

        switch (currentState)
        {
            case NPCState.Patrol:
                HandlePatrolState(seesPlayer);
                break;
            
            case NPCState.Investigate:
                HandleInvestigateState(seesPlayer);
                break;
            
            case NPCState.Chase:
                HandleChaseState(seesPlayer);
                break;
        }
    }

    void HandlePatrolState(bool seesPlayer)
    {
        if (seesPlayer)
        {
            if (player != null)
                lastKnownPlayerPosition = player.position;

            ChangeState(NPCState.Chase);
            return;
        }
        
        movement.Patrol();
    }
    void HandleInvestigateState(bool seesPlayer)
    {
        if (seesPlayer)
        {
            if (player != null)
                lastKnownPlayerPosition = player.position;

            ChangeState(NPCState.Chase);
            return;
        }

        movement.MoveToPoint(investigateTarget, data.investigateSpeed);

        if (!movement.CanReach(investigateTarget))
        {
            Debug.LogWarning($"{name} cannot reach investigate target, returning to patrol");
            ChangeState(NPCState.Patrol);
            return;
        }

        if (movement.ReachedDestination())
        {
            investigateWaitTimer += Time.deltaTime;

            if (investigateWaitTimer >= data.investigateWaitTime)
            {
                ChangeState(NPCState.Patrol);
            }
        }
        else
        {
            investigateWaitTimer = 0f;
        }
    }

    void HandleChaseState(bool seesPlayer)
    {
        if (seesPlayer)
        {
            lostSightTimer = 0f;
            
            if (player != null)
                lastKnownPlayerPosition = player.position;
            
            movement.Chase(data.chaseSpeed);
        }
        else
        {
            lostSightTimer += Time.deltaTime;

            if (lostSightTimer >= data.loseSightDelay)
            {
                investigateTarget = lastKnownPlayerPosition;
                ChangeState(NPCState.Investigate);
            }
        }
    }

    void ChangeState(NPCState newState)
    {
        currentState = newState;
        investigateWaitTimer = 0f;
        lostSightTimer = 0f;
    }

    public void InvestigateLocation(Vector3 worldPosition)
    {
        Vector3 validPoint;

        if (movement.TryGetNearestNavMeshPoint(worldPosition, 12f, out validPoint))
        {
            if (movement.CanReach(validPoint))
            {
                investigateTarget = validPoint;
                Debug.Log($"{name} investigating reachable location: {validPoint}");
                ChangeState(NPCState.Investigate);
            }
            else
            {
                Debug.LogWarning($"{name} found a NavMesh point, but it is not reachable: {validPoint}");
            }
        }
        else
        {
            Debug.LogWarning($"{name} could not find any NavMesh point near {worldPosition}");
        }
    }

    [ContextMenu("Investigate")]
    public void TestInvestigate()
    {
        Vector3 testPosition = transform.position + new Vector3(2f, 0, 0);
        InvestigateLocation(testPosition);
        Debug.Log($"{name} investigating test position: " + testPosition);
    }
    // public NPCData data;
    //
    // private NPCMovement movement;
    // private NPCDetection detection;
    //
    // void Awake()
    // {
    //     movement = GetComponent<NPCMovement>();
    //     detection = GetComponent<NPCDetection>();
    // }
    //
    // void Update()
    // {
    //     if (detection == null || movement == null || data == null)
    //         return;
    //
    //     if (detection.CanSeePlayer())
    //     {
    //         movement.Chase(data.chaseSpeed);
    //     }
    //     else
    //     {
    //         movement.Patrol(); // no parameter needed
    //     }
    //}
}
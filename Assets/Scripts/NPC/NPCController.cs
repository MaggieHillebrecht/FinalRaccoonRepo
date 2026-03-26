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
    public Transform investigateTestPoint;

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

        Debug.DrawLine(transform.position, investigateTarget, Color.yellow, 10f);
        Debug.Log($"{name} → target: {investigateTarget}");

        if (!movement.IsCloseToTarget(investigateTarget, data.investigateArrivalDistance))
        {
            movement.MoveToPoint(investigateTarget, data.investigateSpeed);
            investigateWaitTimer = 0f; // reset wait timer while moving
        }
        else
        {
            movement.StopMoving();
            investigateWaitTimer += Time.deltaTime;
            
            if (investigateWaitTimer >= data.investigateWaitTime)
            {
                ChangeState(NPCState.Patrol);
            }
        }
    }

    void HandleChaseState(bool seesPlayer)
    {
        Debug.Log($"{name} is chasing");

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
                ChangeState(NPCState.Patrol);
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
        investigateTarget = worldPosition;
        ChangeState(NPCState.Investigate);
    }

    public void OnDistractionEvent(Vector3 eventPosition, float eventRange)
    {
        if (currentState == NPCState.Chase)
            return;
        
        float distanceToEvent = Vector3.Distance(transform.position, eventPosition);
        
        if (distanceToEvent <= eventRange)
        {
            InvestigateLocation(eventPosition);
        }
    }

    [ContextMenu("Investigate")]
    public void TestInvestigate()
    {
        if (investigateTestPoint == null)
        {
            Debug.LogWarning($"{name} has no investigateTestPoint assigned");
            return;
        }

        Vector3 testPosition = investigateTestPoint.position;
        InvestigateLocation(testPosition);
    }

    [ContextMenu("Chase")]
    public void TestChase()
    {
        ChangeState(NPCState.Chase);
    }
}
using UnityEngine;
using UnityEngine.XR;

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
        
        movement.Investigate(investigateTarget, data.investigateSpeed);

        if (movement.ReachedDestination())
        {
            investigateWaitTimer += Time.deltaTime;

            if (investigateWaitTimer >= data.investigateWaitTime)
            {
                ChangeState(NPCState.Patrol);
            }
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
        investigateTarget = worldPosition;
        ChangeState(NPCState.Investigate);
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
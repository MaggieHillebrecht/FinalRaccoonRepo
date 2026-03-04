using UnityEngine;

public class NPCController : MonoBehaviour
{
    public NPCData data;

    private NPCMovement movement;
    private NPCDetection detection;

    void Awake()
    {
        movement = GetComponent<NPCMovement>();
        detection = GetComponent<NPCDetection>();
    }

    void Update()
    {
        if (detection == null || movement == null || data == null)
            return;

        if (detection.CanSeePlayer())
        {
            movement.Chase(data.chaseSpeed);
        }
        else
        {
            movement.Patrol(); // no parameter needed
        }
    }
}
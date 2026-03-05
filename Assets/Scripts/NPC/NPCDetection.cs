using UnityEngine;

public class NPCDetection : MonoBehaviour
{
    public float detectionRange = 6f;
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

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= detectionRange;
    }
}
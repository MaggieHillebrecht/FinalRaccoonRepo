using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;

    public float investigateSpeed = 4f;
    public float investigateArrivalDistance = 3f;
    public float investigateWaitTime = 2f;
    
    public float loseSightDelay = 2f;
}

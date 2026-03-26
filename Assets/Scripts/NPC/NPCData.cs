using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public float moveSpeed = 8f;
    public float chaseSpeed = 15f;

    public float investigateSpeed = 10f;
    public float investigateArrivalDistance = 0.5f;
    public float investigateWaitTime = 2f;
    
    public float loseSightDelay = 2f;
}

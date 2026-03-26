using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    //patrol settings
    public float moveSpeed = 8f;
    
    //investigate settings
    public float investigateSpeed = 10f;
    public float investigateArrivalDistance = 0.5f;
    public float investigateWaitTime = 2f;
    
    //chase settings
    public float chaseSpeed = 15f;
    public float detectionRange = 50f;
    public float viewAngle = 170f;
    
    public float loseSightDelay = 4f;
}

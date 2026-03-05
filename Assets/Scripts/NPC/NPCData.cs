using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
}
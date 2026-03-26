using UnityEngine;

public class Distraction : MonoBehaviour
{
    public float eventRange = 25f;

    public void TriggerEvent()
    {
        NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);

        foreach (NPCController npc in npcs)
        {
            npc.OnDistractionEvent(transform.position, eventRange);
        }
    }

    [ContextMenu("Trigger Distraction")]
    public void TestTriggerEvent()
    {
        TriggerEvent();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, eventRange);
    }
}

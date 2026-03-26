using UnityEngine;

public class BreakableObject : MonoBehaviour, IInteractable
{
    public Distraction distraction;
    public bool isBroken = false;

    public void Interact(GameObject player)
    {
        if (isBroken)
            return;

        BreakObject();
    }

    void BreakObject()
    {
        Debug.Log($"{name} broke");
        isBroken = true;

        if (distraction != null)
        {
            distraction.TriggerEvent();
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        gameObject.SetActive(false);
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();

        if (input == null) 
            return "";

        string key = input.GetInteractKey();

        if (isBroken)
            return "Already broken";
        return $"[{key}] Break";
    }
}

using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        Debug.Log("Pickup object");

        Rigidbody rb = GetComponent<Rigidbody>();

        if (!rb) return;

        Transform holdPos = player.GetComponent<PlayerInteraction>().transform;

        rb.isKinematic = true;
        transform.SetParent(holdPos);
        transform.localPosition = Vector3.forward;
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();

        if (input == null || state == null) return "";

        string key = input.GetInteractKey();

        return $"{key} to pick up";
    }
}

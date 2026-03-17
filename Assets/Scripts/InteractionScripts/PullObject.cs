using UnityEngine;

public class PullObject : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        Debug.Log("Pull object");

        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (!rb || !playerRb) return;

        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = playerRb;
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();

        if (input == null || state == null) return "";

        string key = input.GetInteractKey();

        return $"{key} to pull";
    }
}
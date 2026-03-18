using UnityEngine;

public class PullObject : MonoBehaviour, IInteractable
{
    private ConfigurableJoint joint;

    public void Interact(GameObject player)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();

        if (!rb || !playerRb || state == null) return;

        if (joint != null)
        {
            StopPulling(state);
        }
        else
        {
            StartPulling(playerRb, state);
        }
    }

    void StartPulling(Rigidbody playerRb, PlayerInteractionState state)
    {
        Debug.Log("[PULL] Start Pulling");

        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = playerRb;

        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 1f; // distance from player
        joint.linearLimit = limit;

        state.StartPulling();
    }

    void StopPulling(PlayerInteractionState state)
    {
        Debug.Log("[PULL] Stop Pulling");

        if (joint != null)
            Destroy(joint);

        state.StopPulling();
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        PlayerInteractionState state = player.GetComponent<PlayerInteractionState>();

        if (input == null || state == null) return "";

        string key = input.GetInteractKey();

        if (joint != null)
            return $"[{key}] Stop Pulling";

        return $"[{key}] Pull";
    }
}
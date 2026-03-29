using UnityEngine;
using TMPro;

public class PullObject : MonoBehaviour, IInteractable
{
    [Header("UI & Highlight")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Outline outline;
    [SerializeField] private float highlightRange = 3f;

    private GameObject player;
    private ConfigurableJoint joint;
    private PlayerInteractionState playerState;

    private void Awake()
    {
        if (outline != null) outline.enabled = false;
        if (interactText != null) interactText.gameObject.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool inRange = distance <= highlightRange;

            if (outline != null) outline.enabled = inRange;
            if (interactText != null)
            {
                interactText.gameObject.SetActive(inRange);
                if (inRange) interactText.text = GetInteractText(player);
            }
        }
    }

    public void Interact(GameObject player)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        playerState = player.GetComponent<PlayerInteractionState>();

        if (!rb || !playerRb || playerState == null) return;

        if (joint != null)
            StopPulling();
        else
            StartPulling(playerRb);
    }

    private void StartPulling(Rigidbody playerRb)
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = playerRb;

        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit { limit = 0.5f };
        joint.linearLimit = limit;

        Vector3 dir = (transform.position - playerRb.transform.position).normalized;
        playerState.StartPulling(dir);
    }

    private void StopPulling()
    {
        if (joint != null) Destroy(joint);
        joint = null;

        if (playerState != null) playerState.StopPulling();
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        if (input == null) return "";

        string key = input.GetInteractKey();
        return (joint != null) ? $"[{key}] Stop Pulling" : $"[{key}] Pull";
    }

    private void OnDrawGizmosSelected()
    {
        if (highlightRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, highlightRange);
        }
    }
}
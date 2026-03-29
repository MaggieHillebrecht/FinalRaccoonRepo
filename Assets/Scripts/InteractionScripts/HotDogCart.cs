using UnityEngine;
using TMPro;

public class HotDogCart : MonoBehaviour, IInteractable
{
    [Header("UI & Highlight")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Outline outline;
    [SerializeField] private float highlightRange = 3f;

    private GameObject player;

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
        GameStateController.Instance.WinGame();
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();
        if (input == null) return "";

        string key = input.GetInteractKey();
        return $"[{key}] Eat Hotdog";
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
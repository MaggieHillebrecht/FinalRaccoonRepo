using UnityEngine;
using System.Linq;

public class NPCCollisionHandler : MonoBehaviour
{
    private GameObject losePanel;

    void Start()
    {
        losePanel = Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(obj => obj.name == "LoseScreen");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!GameStateController.IsGameOver)
            {
                GameStateController.Instance.GameOver();

                if (losePanel != null)
                    losePanel.SetActive(true);
            }
        }
    }
}
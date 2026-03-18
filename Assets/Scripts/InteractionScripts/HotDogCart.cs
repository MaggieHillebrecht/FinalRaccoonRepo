using UnityEngine;

public class HotDogCart : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        GameStateController.Instance.WinGame();
    }

    public string GetInteractText(GameObject player)
    {
        PlayerInputReader input = player.GetComponent<PlayerInputReader>();

        if (input == null) return "";

        string key = input.GetInteractKey();

        return $"{key} to eat hotdog";
    }
}
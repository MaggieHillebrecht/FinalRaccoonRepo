using UnityEngine;
using UnityEngine.InputSystem;

public class ActivatingMap : MonoBehaviour
{
    [SerializeField] GameObject mapPanel;

    PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMap(InputValue value)
    {
        if (value.isPressed)
        {
            OpenMap();
        }
        else
        {
            CloseMap();
        }
    }

    public void OpenMap()
    {
        mapPanel.SetActive(true);
        GameStateController.Instance.OpenMap();
    }

    public void CloseMap()
    {
        mapPanel.SetActive(false);
        GameStateController.Instance.CloseMap();
    }
}
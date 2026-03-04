using UnityEngine;

public class ActivatingMap : MonoBehaviour
{
    [SerializeField] GameObject mapPanel;
    [SerializeField] PlayerInputReader input;

    void OnEnable()
    {
        if (input != null)
            input.OnMapPressed += ToggleMap;
    }

    void OnDisable()
    {
        if (input != null)
            input.OnMapPressed -= ToggleMap;
    }

    public void ToggleMap()
    {
        if (GameStateController.IsMapOpen)
            CloseMap();
        else
            OpenMap();
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
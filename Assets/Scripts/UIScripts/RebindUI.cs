using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
    public PlayerInput playerInput;
    public string actionName;
    public TMP_Text bindingDisplayText;
    public Button rebindButton;

    void Start()
    {
        UpdateBindingDisplay();
        rebindButton.onClick.AddListener(StartRebind);
    }

    void UpdateBindingDisplay()
    {
        var action = playerInput.actions[actionName];

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].effectivePath.Contains("Keyboard"))
            {
                bindingDisplayText.text =
                    InputControlPath.ToHumanReadableString(
                        action.bindings[i].effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice
                    );

                return;
            }
        }

        bindingDisplayText.text = "No Keyboard Binding";
    }

    public void StartRebind()
    {
        var action = playerInput.actions[actionName];

        int bindingIndex = -1;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].effectivePath.Contains("Keyboard"))
            {
                bindingIndex = i;
                break;
            }
        }

        if (bindingIndex == -1)
        {
            return;
        }

        rebindButton.interactable = false;
        action.Disable();
        bindingDisplayText.text = "Press a key...";

        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                operation.Dispose();
                action.Enable();
                UpdateBindingDisplay();
                rebindButton.interactable = true;
            })
            .Start();
    }
}
using UnityEngine;

public class PlayerInteractionState : MonoBehaviour
{
    public bool IsHolding { get; private set; }
    public bool IsPulling { get; private set; }

    public void StartHolding() => IsHolding = true;
    public void StopHolding() => IsHolding = false;

    public void StartPulling() => IsPulling = true;
    public void StopPulling() => IsPulling = false;
}

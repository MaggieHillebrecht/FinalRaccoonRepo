using UnityEngine;

public class PlayerInteractionState : MonoBehaviour
{
    public bool IsHolding { get; private set; }
    public bool IsPulling { get; private set; }
    public bool IsHidden { get; private set; }
    public bool IsMovementBlocked { get; private set; }

    public void StartHolding() => IsHolding = true;
    public void StopHolding() => IsHolding = false;

    public void StartPulling() => IsPulling = true;
    public void StopPulling() => IsPulling = false;

    public void EnterHide()
    {
        Debug.Log("[STATE] EnterHide");
        IsHidden = true;
        BlockMovement();
    }

    public void ExitHide()
    {
        Debug.Log("[STATE] ExitHide");
        IsHidden = false;
        UnblockMovement();
    }

    public void BlockMovement()
    {
        Debug.Log("[STATE] Movement BLOCKED");
        IsMovementBlocked = true;
    }

    public void UnblockMovement()
    {
        Debug.Log("[STATE] Movement UNBLOCKED");
        IsMovementBlocked = false;
    }
}
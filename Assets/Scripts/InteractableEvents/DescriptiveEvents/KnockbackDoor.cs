using UnityEngine;

public class KnockbackDoor : DescriptiveInteractable, IInteractable
{
    private Level1Flags _level1Flags;
    protected new void Start()
    {
        base.Start();
        _level1Flags = FindFirstObjectByType<Level1Flags>();
    }
    
    public new void AttemptToInteract()
    {
        base.AttemptToInteract();
        _level1Flags.IsEventDoorKnocked = true;
    }
}

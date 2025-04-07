using UnityEngine;

public class Level1Flags : MonoBehaviour
{
    public bool IsEventDoorKnocked { get; set; }

    private void Start()
    {
        IsEventDoorKnocked = false;
    }
}

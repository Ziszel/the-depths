using System.Collections;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour
{
    [SerializeField] protected float movementDuration;
    [SerializeField] protected bool isOpen;
    [SerializeField] protected bool isKeyPowered;
    [SerializeField] protected InventoryItem keyItem;
    [SerializeField] protected bool lockedFromOtherSideDoor;
    [SerializeField] protected bool updateGoalTextOnOpenOnly;
    [SerializeField] protected GameObject doorCollisionHelperOpen;
    [SerializeField] protected GameObject doorCollisionHelperLocked;
    [SerializeField] protected GameObject childDoorMeshObject;
    
    protected DoorAudio DoorAudio;
    protected GameObject DoorMesh; // Used to stop interactivity after interaction
    
    protected abstract IEnumerator OpenDoor(float dot);
}

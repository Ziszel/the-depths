using System.Collections;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour
{
    [SerializeField] protected float movementDuration;
    
    protected DoorAudio DoorAudio;
    protected GameObject DoorMesh; // Used to stop interactivity after interaction
    protected bool IsOpen;
    
    protected abstract IEnumerator OpenDoor();
}

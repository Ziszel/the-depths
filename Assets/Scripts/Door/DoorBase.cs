using System.Collections;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour
{
    [SerializeField] protected float movementDuration;
    
    protected DoorAudio _doorAudio;
    
    protected bool IsOpen;
    
    protected abstract IEnumerator OpenDoor();
}

using System.Collections;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class RotatingDoor : DoorBase, ISwitchable
{
    // Rotate by this amount of degrees over movementDuration
    [SerializeField] private Vector3 rotateByDegrees = new Vector3(0.0f, 0.0f, 0.0f);
    
    private void Start()
    {
        _doorAudio = GetComponent<DoorAudio>();
    }
    
    protected override IEnumerator OpenDoor()
    {
        float timeElapsed = 0.0f;
        var endRotation = transform.rotation * Quaternion.Euler(rotateByDegrees);
        
        while (timeElapsed < movementDuration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, timeElapsed / movementDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = endRotation;
        Debug.Log("Door is opened");
    }
    
    public void Toggle()
    {
        if (!IsOpen)
        {
            _doorAudio.PlaySfx();
            StartCoroutine(OpenDoor());
            IsOpen = true;
        }
    }
}

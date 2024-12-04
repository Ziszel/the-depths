using System.Collections;
using UnityEngine;

public class RotatingDoor : DoorBase, ISwitchable
{
    // Rotate by this amount of degrees over movementDuration
    [SerializeField] private Vector3 rotateByDegrees = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private float rotationSpeed;

    private Quaternion initialRotation;
    private Quaternion endRotation;
    
    private void Start()
    {
        initialRotation = transform.rotation;
        endRotation = Quaternion.Inverse(initialRotation) * Quaternion.Euler(rotateByDegrees);
        _doorAudio = GetComponent<DoorAudio>();
    }

    protected override IEnumerator OpenDoor()
    {
        
        while (Quaternion.Angle(initialRotation, endRotation) > 0.01f)
        {
            // returns a quaternion rotated towards endRotation by the step value
            // Updates initialRotation since the next frame will rotate from that point
            initialRotation = Quaternion.RotateTowards(initialRotation, endRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = initialRotation;
            yield return null;
        }

        transform.rotation = endRotation;
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

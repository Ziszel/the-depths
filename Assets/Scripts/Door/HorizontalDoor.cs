using System;
using System.Collections;
using UnityEngine;

public class HorizontalDoor : DoorBase, ISwitchable
{
    [SerializeField] private Vector3 movementAmount = new(0.0f, 0.0f, 30.0f);
    
    private void Start()
    {
        _doorAudio = GetComponent<DoorAudio>();
    }

    protected override IEnumerator OpenDoor()
    {
        float timeElapsed = 0.0f;
        Vector3 startPosition = transform.position;
        Vector3 finalPosition = transform.position + movementAmount;

        while (timeElapsed < movementDuration)
        {
            transform.position = Vector3.Lerp(startPosition, finalPosition, timeElapsed / movementDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;
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

using System.Collections;
using UnityEngine;

public class HorizontalDoor : DoorBase, ISwitchable
{
    [SerializeField] private Vector3 movementAmount = new(0.0f, 0.0f, 30.0f);
    
    private void Start()
    {
        DoorAudio = GetComponent<DoorAudio>();
    }

    // Horizontal door does not need to use dot parameter so pass in 0.0f
    protected override IEnumerator OpenDoor(float dot)
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
        if (!isOpen)
        {
            DoorAudio.PlaySfx();
            StartCoroutine(OpenDoor(0.0f));
            isOpen = true;
        }
    }
}

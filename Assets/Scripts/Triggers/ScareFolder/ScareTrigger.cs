using UnityEngine;

public class ScareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] scareEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            foreach (var se in scareEvents)
            {
                if (se.TryGetComponent(out IScareEvent scareEvent))
                {
                    scareEvent.TriggerScareEvent();
                }
            }
            gameObject.SetActive(false);
        }
    }
}

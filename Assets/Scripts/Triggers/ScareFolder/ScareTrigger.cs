using UnityEngine;

public class ScareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] _scareEvents;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var se in _scareEvents)
        {
            if (se.TryGetComponent(out IScareEvent scareEvent))
            {
                scareEvent.TriggerScareEvent();
            }
        }
    }
}

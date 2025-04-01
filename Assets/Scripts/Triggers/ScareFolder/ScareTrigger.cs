using UnityEngine;

public class ScareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _scareEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (_scareEvent.TryGetComponent(out IScareEvent scareEvent))
        {
            scareEvent.TriggerScareEvent();
        }
    }
}

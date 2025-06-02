using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScareTrigger : MonoBehaviour, IResettable
{
    [SerializeField] private GameObject[] scareEventObjects;
    [SerializeField] private bool shouldPlayMultipleOnSameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            foreach (var obj in scareEventObjects)
            {
                if (shouldPlayMultipleOnSameObject)
                {
                    List<IScareEvent> childScareEvents = obj.GetComponents<IScareEvent>().ToList();

                    foreach (var scareEvent in childScareEvents)
                    {
                        scareEvent.TriggerScareEvent();
                    }
                }
                else
                {
                    if (obj.TryGetComponent(out IScareEvent scareEvent))
                    {
                        scareEvent.TriggerScareEvent();
                    }
                }
            }

            enabled = false;
        }
    }

    public void ResetObjectState()
    {
        Debug.Log("IScareTrigger Reset!");
        enabled = true;
    }
}

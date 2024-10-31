using UnityEngine;

public class LightProximitySensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            if (other.TryGetComponent(out IMonsterInteractable monsterInteractable))
            {
                monsterInteractable.AffectedByMonster();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            if (other.TryGetComponent(out IMonsterInteractable monsterInteractable))
            {
                monsterInteractable.StopAffectedByMonster();
            }
        }
    }
}

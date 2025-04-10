using System;
using UnityEngine;

public class UpdateGoalTextTrigger : MonoBehaviour
{
    private UpdatePlayerGoalText _updatePlayerStatusText;

    private void Start()
    {
        _updatePlayerStatusText = GetComponent<UpdatePlayerGoalText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            _updatePlayerStatusText.UpdateStatusText();
            Destroy(gameObject);
        }
    }
}

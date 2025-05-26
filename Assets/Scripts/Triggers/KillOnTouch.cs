using UnityEngine;

public class KillOnTouch : MonoBehaviour
{
    PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            player.OnKillPlayer();
        }
    }
}

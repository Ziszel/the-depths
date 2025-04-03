using UnityEngine;

public class EndGame : MonoBehaviour
{
    private LevelManager _levelManager;

    public void Start()
    {
        _levelManager = FindAnyObjectByType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            if (!_levelManager.IsPlayerDead())
            {
                InputManager.ToggleActionMap(InputManager.PlayerInputActions.Hiding);
                GameManager.Instance.SetBestTime(LevelManager.GetTimer());
                GameManager.Instance.LoadLevel("EndGame");
            }
        }
    }
}

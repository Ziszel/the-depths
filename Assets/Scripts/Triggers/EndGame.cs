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
        if (other.TryGetComponent(out PlayerController player))
        {
            Debug.Log("Player entered");
            if (!_levelManager.IsPlayerDead())
            {
                GameManager.Instance.SetBestTime(LevelManager.GetTimer());
                GameManager.Instance.LoadLevel("EndGame");
            }
        }
    }
}

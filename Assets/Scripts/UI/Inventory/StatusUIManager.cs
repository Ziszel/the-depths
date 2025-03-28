using TMPro;
using UnityEngine;

public class StatusUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text deathCountText;
    [SerializeField] private TMP_Text saveCountText;

    public void InitialiseStatusScreen()
    {
        playTimeText.text = LevelManager.GetTimerAsString();
        deathCountText.text = GameManager.Instance.GetDeathCount().ToString();
        saveCountText.text = GameManager.Instance.GetSaveCount().ToString();
    }

    private void Update()
    {
        playTimeText.text = LevelManager.GetTimerAsString();
    }
}

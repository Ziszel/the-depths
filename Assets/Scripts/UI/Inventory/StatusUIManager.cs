using TMPro;
using UnityEngine;

public class StatusUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text deathCountText;
    [SerializeField] private TMP_Text saveCountText;

    public void InitialiseStatusScreen()
    {
        playTimeText.text = GameManager.instance.GetCurrentPlayTimeAsString();
        deathCountText.text = GameManager.instance.GetDeathCount().ToString();
        saveCountText.text = GameManager.instance.GetSaveCount().ToString();
    }

    private void Update()
    {
        playTimeText.text = GameManager.instance.GetCurrentPlayTimeAsString();
    }
}

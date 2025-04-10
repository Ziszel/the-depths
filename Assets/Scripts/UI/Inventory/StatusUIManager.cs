using TMPro;
using UnityEngine;

public class StatusUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private TMP_Text deathCountText;
    [SerializeField] private TMP_Text saveCountText;
    
    private Inventory _inventory;

    public void InitialiseStatusScreen()
    {
        playTimeText.text = LevelManager.GetTimerAsString();
        goalText.text = _inventory.GetCurrentGoalText();
        deathCountText.text = GameManager.Instance.GetDeathCount().ToString();
        saveCountText.text = GameManager.Instance.GetSaveCount().ToString();
    }

    private void Update()
    {
        playTimeText.text = LevelManager.GetTimerAsString();
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }
}

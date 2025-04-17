using UnityEngine;

public class UpdatePlayerGoalTextDoor : UpdatePlayerGoalText
{
    [SerializeField] private string newGoalTextLocked;

    public void UpdateStatusText(bool isDoorLocked, bool shouldUpdateOnOpenOnly)
    {
        if (!ArePrerequisitesMet())
        {
            return;
        }

        if (isDoorLocked)
        {
            if (!shouldUpdateOnOpenOnly)
            {
                LevelManager.GetInventoryFromPlayer().SetCurrentGoalText(newGoalTextLocked);
                UpdateJournalText();
            }
        }
        else
        {
            LevelManager.GetInventoryFromPlayer().SetCurrentGoalText(newStatusText);
            UpdateJournalText();
        }
    }
}

using System;
using UnityEngine;

public class UpdatePlayerGoalText : MonoBehaviour
{
    public static event Action OnJournalTextUpdateFromText;
    
    [SerializeField] protected string newStatusText;
    [SerializeField] private UpdatePlayerGoalText[] prerequisiteStatusTexts;

    public void UpdateStatusText()
    {
        if (!ArePrerequisitesMet())
        {
            return;
        }
        
        LevelManager.GetInventoryFromPlayer().SetCurrentGoalText(newStatusText);
        UpdateJournalText();
        enabled = false;
    }

    protected bool ArePrerequisitesMet()
    {
        if (prerequisiteStatusTexts.Length > 0)
        {
            foreach (var pStatustext in prerequisiteStatusTexts)
            {
                if (pStatustext.enabled)
                {
                    // Do not trigger this status text if pre-requisites are not met!
                    return false;
                }
            }
        }
        return true;
    }
    
    protected void UpdateJournalText()
    {
        OnJournalTextUpdateFromText?.Invoke();
    }
}

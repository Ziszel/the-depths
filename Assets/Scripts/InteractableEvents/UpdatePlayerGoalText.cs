using UnityEngine;

public class UpdatePlayerGoalText : MonoBehaviour
{
    [SerializeField] protected string newStatusText;
    [SerializeField] private UpdatePlayerGoalText[] prerequisiteStatusTexts;
    
    protected LevelManager LevelManager;

    private void Start()
    {
        LevelManager = FindFirstObjectByType<LevelManager>();
    }

    public void UpdateStatusText()
    {
        if (!ArePrerequisitesMet())
        {
            return;
        }
        
        LevelManager.GetInventoryFromPlayer().SetCurrentGoalText(newStatusText); 
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
}

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private GameObject[] resettableObjects;
    
    public void ResetObjectsInLevel()
    {
        foreach (GameObject obj in resettableObjects)
        {
            Debug.Log(obj.name);
            if (TryGetComponent(out IResettable resettableObject))
            {
                resettableObject.ResetObjectState();
            }
        }
        
        // Reset global values that do not implement IResettable
        GameManager.Instance.musicManager.StopMusic();
    }
}

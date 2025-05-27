using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private GameObject[] resettableObjects;
    
    private string _checkpointGoalText;
    private List<InventoryItem> _checkpointItemData;
    private List<FileData> _checkpointfileData;
    public Transform respawnTransform;

    public void SetPlayerCheckpointValues(Vector3 respawnPosition, Vector3 respawnRotation, 
        Inventory currentInventory)
    {
        _checkpointGoalText = currentInventory.GetCurrentGoalText();
        _checkpointItemData = currentInventory.GetItems();
        _checkpointfileData = currentInventory.GetFileData();
        respawnTransform.position = respawnPosition;
        respawnTransform.rotation = Quaternion.Euler(respawnRotation);
    }

    public void RestorePlayerCheckpointValues(PlayerController player)
    {
        // Inventory
        player.GetPlayerInventory().ResetToCheckpointData(_checkpointGoalText,
            _checkpointItemData,
            _checkpointfileData);
        player.transform.position = respawnTransform.position;
        CameraManager.ForceCurrentCameraRotation(Quaternion.LookRotation(respawnTransform.forward, Vector3.up));
    }
    
    public void ResetObjectsInLevel()
    {
        foreach (GameObject obj in resettableObjects)
        {
            // TODO: Investigate whether or not this is actually required.
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;
            }
            
            if (obj.TryGetComponent(out IResettable resettableObject))
            {
                resettableObject.ResetObjectState();
            }
        }
        
        // Reset global values that do not implement IResettable
        GameManager.Instance.musicManager.StopMusic();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> resettableObjects;
    
    private string _checkpointGoalText;
    private List<InventoryItem> _checkpointItemData;
    private List<FileData> _checkpointfileData;
    public Transform respawnTransform;

    public void SetPlayerCheckpointValues(Vector3 respawnPosition, Vector3 respawnRotation, 
        Inventory currentInventory)
    {
        _checkpointGoalText = currentInventory.GetCurrentGoalText();
        _checkpointItemData = new List<InventoryItem>(currentInventory.GetItems());
        _checkpointfileData = new List<FileData>(currentInventory.GetFileData());
        respawnTransform.position = respawnPosition;
        respawnTransform.rotation = Quaternion.Euler(respawnRotation);
        RemoveResettableObjects();
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

    // If a player has collected an item, we don't want it to respawn.
    // If a player has already triggered a switch, we don't want it to reset
    // etc... By removing the resettable object from the list, this can be avoided
    // Called when a new checkpoint is reached so that parameters can be checked
    private void RemoveResettableObjects()
    {
        // Remove items already in the players inventory
        for (int i = resettableObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = resettableObjects[i];
            
            if (obj.TryGetComponent(out Pickup pickup))
            {
                if (pickup.itemData.GetType() == typeof(InventoryItem))
                {
                    InventoryItem invItem = (InventoryItem)pickup.itemData;
                    if (RemoveInventoryItemIfCollected(invItem))
                    {
                        resettableObjects.RemoveAt(i);
                    }
                }

                if (pickup.itemData.GetType() == typeof(FileItem))
                {
                    FileItem fileItem = (FileItem)pickup.itemData;
                    if (RemoveFileItemIfCollected(fileItem))
                    {
                        resettableObjects.RemoveAt(i);
                    }
                }
            }
            
        }
    }

    private bool RemoveInventoryItemIfCollected(InventoryItem invItem)
    {
        for (int i = _checkpointItemData.Count - 1; i >= 0; i--)
        {
            if (_checkpointItemData[i].itemName.Equals(invItem.itemName))
            {
                return true;
            }
        }
        return false;
    }

    private bool RemoveFileItemIfCollected(FileItem fileItem)
    {
        for (int i = _checkpointfileData.Count - 1; i >= 0; i--)
        {
            if (_checkpointfileData[i].id == fileItem.fileId)
            {
                return true;
            }
        }
        return false;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> _items;
    private string _currentGoalText;
    private List<FileData> _foundFileData;
    
    private FileDataManager _fileDataManager;

    private void Start()
    {
        _items = new List<InventoryItem>();
        _foundFileData = new List<FileData>();
        // Not sure this is the best way of handling this but will do for now
        _fileDataManager = FindFirstObjectByType<FileDataManager>();
        
        // If moving between scenes, make sure to update the new Inventory object
        // with the stored values of items, files, and goalText
        if (GameManager.Instance.GetSavedPlayerItems().Count > 0)
        {
            _items = GameManager.Instance.GetSavedPlayerItems();
        }
        
        if (GameManager.Instance.GetSavedFileData().Count > 0)
        {
            _foundFileData = GameManager.Instance.GetSavedFileData();
        }

        if (string.IsNullOrEmpty(GameManager.Instance.GetCurrentGoalText()))
        {
            _currentGoalText = GameManager.Instance.GetInitialGoalText();
        }
        else
        {
            _currentGoalText = GameManager.Instance.GetCurrentGoalText();
        }
    }

    // InventoryItems operations
    public void AddItem(InventoryItem item)
    {
        Debug.Log(item.name);
        _items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        _items.Remove(item);
    }

    public void UseItem(InventoryItem item)
    {
        item.UseItem();
    }

    public List<InventoryItem> GetItems()
    {
        return _items;
    }

    public bool ContainsItem(InventoryItem item)
    {
        return _items.Contains(item);
    }

    public InventoryItem GetItemByIndex(int index)
    {
        return _items[index];
    }
    
    public string GetCurrentGoalText()
    {
        return _currentGoalText;
    }

    public void SetCurrentGoalText(string text)
    {
        _currentGoalText = text;
    }

    // Files operations
    public void AddFile(FileItem newFile)
    {
        FileData? fd;
        _fileDataManager.GetFileByIndex(newFile.fileId, out fd);
        _foundFileData.Add(fd.Value);
    }

    public List<FileData> GetFileData()
    {
        return _foundFileData;
    }

    public FileData? GetFileDataByIndex(int index)
    {
        foreach (FileData fd in _foundFileData)
        {
            if (fd.id == index)
            {
                return fd;
            }
        }

        return null;
    }
}

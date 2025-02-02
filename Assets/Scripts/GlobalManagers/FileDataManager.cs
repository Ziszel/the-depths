using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class FileDataManager : MonoBehaviour
{
    private List<FileData> _fileDatasOnDisk; // store all fileData from file on load
    
    private void Start()
    {
        _fileDatasOnDisk = new List<FileData>();
        LoadFilesFromDisk();
    }
    
    // Does not order the files
    private void LoadFilesFromDisk()
    {
        string filesFolder = Application.dataPath + "/FileJSON";
        
        DirectoryInfo d = new DirectoryInfo(filesFolder);
        foreach (var file in d.GetFiles("*.json"))
        {
            string json = File.ReadAllText(file.FullName);
            FileData fileData = JsonUtility.FromJson<FileData>(json);
            _fileDatasOnDisk.Add(fileData);
        }
    }

    // this function is designed to always return a file
    public bool GetFileByIndex(int index, out FileData? fileData)
    {
        foreach (var fd in _fileDatasOnDisk)
        {
            if (fd.id == index)
            {
                fileData = fd;
                return true;
            }
        }
        fileData = null;
        return false;
    }
}

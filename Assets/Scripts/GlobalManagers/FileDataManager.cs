using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class FileDataManager : MonoBehaviour
{
    private List<FileData> _fileDatasOnDisk; // store all fileData from file on load
    
    void Start()
    {
        _fileDatasOnDisk = new List<FileData>();
        LoadFilesFromDisk();
        
        // test
        Debug.Log(_fileDatasOnDisk[0].name);
        Debug.Log(_fileDatasOnDisk[0].content);
    }
    
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
}

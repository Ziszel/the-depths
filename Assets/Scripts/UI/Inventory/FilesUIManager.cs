using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilesUIManager : MonoBehaviour
{
    [SerializeField] private FileReader fileReader;
    
    // List of buttons that load UI
    public Button File01;
    public Button File02;
    public Button File03;
    public Button File04;
    public Button File05;
    public Button File99;
    
    private Inventory _inventory;

    private void Start()
    {
        File01.onClick.AddListener(OnFile01Pressed);
        File02.onClick.AddListener(OnFile02Pressed);
        File03.onClick.AddListener(OnFile03Pressed);
        File04.onClick.AddListener(OnFile04Pressed);
        File05.onClick.AddListener(OnFile05Pressed);
        File99.onClick.AddListener(OnFile99Pressed);
    }

    // update the list of files so that those that are found are renamed from '???'.
    public void InitialiseFiles()
    {
        foreach (var file in _inventory.GetFileData())
        {
            switch (file.id)
            {
                case 1:
                    File01.GetComponentInChildren<TMP_Text>().text = file.name;
                    File01.interactable = true;
                    break;
                case 2:
                    File02.GetComponentInChildren<TMP_Text>().text = file.name;
                    File02.interactable = true;
                    break;
                case 3:
                    File03.GetComponentInChildren<TMP_Text>().text = file.name;
                    File03.interactable = true;
                    break;
                case 4:
                    File04.GetComponentInChildren<TMP_Text>().text = file.name;
                    File04.interactable = true;
                    break;
                case 5:
                    File05.GetComponentInChildren<TMP_Text>().text = file.name;
                    File05.interactable = true;
                    break;
                case 99:
                    File99.GetComponentInChildren<TMP_Text>().text = file.name;
                    File99.interactable = true;
                    break;
                default:
                    break;
            }
        }
    }
    
    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }
    
    // Hook up buttons
    private void OnFile01Pressed()
    {
        FileData? fd = _inventory.GetFileDataByIndex(1);

        if (fd.HasValue)
        {
            fileReader.gameObject.SetActive(true);
            fileReader.SetupPageData(fd.Value.content, fd.Value.name);
        }
    }
    
    private void OnFile02Pressed()
    {
        FileData? fd = _inventory.GetFileDataByIndex(2);

        if (fd.HasValue)
        {
            fileReader.gameObject.SetActive(true);
            fileReader.SetupPageData(fd.Value.content, fd.Value.name);
        }
    }
    
    private void OnFile03Pressed()
    {
        FileData? fd = _inventory.GetFileDataByIndex(3);

        if (fd.HasValue)
        {
            fileReader.gameObject.SetActive(true);
            fileReader.SetupPageData(fd.Value.content, fd.Value.name);
        }
    }
    
    private void OnFile04Pressed()
    {
        FileData? fd = _inventory.GetFileDataByIndex(4);

        if (fd.HasValue)
        {
            fileReader.gameObject.SetActive(true);
            fileReader.SetupPageData(fd.Value.content, fd.Value.name);
        }
    }
    
    private void OnFile05Pressed()
    {
        FileData? fd = _inventory.GetFileDataByIndex(5);

        if (fd.HasValue)
        {
            fileReader.gameObject.SetActive(true);
            fileReader.SetupPageData(fd.Value.content, fd.Value.name);
        }
    }
    
    private void OnFile99Pressed()
    {
        FileData? fd = _inventory.GetFileDataByIndex(99);

        if (fd.HasValue)
        {
            fileReader.EnableFileReaderUI();
            fileReader.SetupPageData(fd.Value.content, fd.Value.name);
        }
    }
}

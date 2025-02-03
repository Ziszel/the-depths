using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilesUIManager : MonoBehaviour
{
    // List of buttons that load UI
    public Button File01;
    public Button File02;
    public Button File03;
    public Button File04;
    public Button File05;
    
    private Inventory _inventory;

    private void Start()
    {
        File01.onClick.AddListener(OnFile01Pressed);
        File02.onClick.AddListener(OnFile02Pressed);
        File03.onClick.AddListener(OnFile03Pressed);
        File04.onClick.AddListener(OnFile04Pressed);
        File05.onClick.AddListener(OnFile05Pressed);
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
        Debug.Log("OnFile01Pressed");
    }
    
    private void OnFile02Pressed()
    {
        Debug.Log("OnFile02Pressed");
    }
    
    private void OnFile03Pressed()
    {
        Debug.Log("OnFile03Pressed");
    }
    
    private void OnFile04Pressed()
    {
        Debug.Log("OnFile04Pressed");
    }
    
    private void OnFile05Pressed()
    {
        Debug.Log("OnFile05Pressed");
    }
}

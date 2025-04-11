using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileReader : MonoBehaviour
{
    [SerializeField] private Image backgroundImageLetter;
    [SerializeField] private Image backgroundImageBlack;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text textBlockText;
    [SerializeField] private TMP_Text pageCountText;
    [SerializeField] private Button nextTextBlockBtn;
    [SerializeField] private Button closeFileReaderBtn;
    [SerializeField] private Button previousTextBlockBtn;
    
    private int _pageCount;
    private int _currentPage;
    private List<string> _pageData;
    
    // External classes
    GlobalSFXPlayer _globalSfxPlayer;
    
    void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        nextTextBlockBtn.onClick.AddListener(OnNextTextBlockPressed);
        previousTextBlockBtn.onClick.AddListener(OnPreviousTextBlockPressed);
        closeFileReaderBtn.onClick.AddListener(OnCloseFileReaderPressed);
    }

    private void OnNextTextBlockPressed()
    {
        _currentPage++;
        UpdatePageCount();
        UpdateTextBlock();

        if (_currentPage == _pageCount)
        {
            nextTextBlockBtn.gameObject.SetActive(false);
            closeFileReaderBtn.gameObject.SetActive(true);
        }
        
        previousTextBlockBtn.interactable = true;
        _globalSfxPlayer.PlaySfx(_globalSfxPlayer.fileReaderPageChange);
    }

    private void OnCloseFileReaderPressed()
    {
        DisableFileReaderUI();
    }
    
    private void OnPreviousTextBlockPressed()
    {
        _currentPage--;
        UpdatePageCount();
        UpdateTextBlock();
        
        if (_currentPage == 1)
        {
            previousTextBlockBtn.interactable = false;
        }
        
        nextTextBlockBtn.gameObject.SetActive(true);
        closeFileReaderBtn.gameObject.SetActive(false);
        _globalSfxPlayer.PlaySfx(_globalSfxPlayer.fileReaderPageChange);
    }

    private void UpdateTextBlock()
    {
        textBlockText.text = _pageData[_currentPage - 1];
    }

    private void UpdatePageCount()
    {
        pageCountText.text = $"{_currentPage}/{_pageCount}";
    }

    public void SetupPageData(string message, string title)
    {
        Debug.Log("SetupPageData");
        _pageData = new List<string>(); // double check this is ok even if it works
        
        // NOTE: If a string looks incorrect, then you need to fix it at the JSON level.
        // This avoids slow processing via code + gives complete control over how the text
        // is presented (even if it can be slightly tedious at first).
        string[] splitString = message.Split('|');
        foreach (string str in splitString)
        {
            _pageData.Add(str);
        }
        
        if (_pageData.Count > 1)
        {
            _pageCount = _pageData.Count;
            nextTextBlockBtn.gameObject.SetActive(true);
            nextTextBlockBtn.interactable = true;
            closeFileReaderBtn.gameObject.SetActive(false);
        }
        else
        {
            _pageCount = 1;
            nextTextBlockBtn.gameObject.SetActive(false);
            closeFileReaderBtn.gameObject.SetActive(true);
        }
        
        previousTextBlockBtn.interactable = false;
        _currentPage = 1;
        titleText.text = title;
        
        UpdateTextBlock();
        UpdatePageCount();
    }

    public void EnableFileReaderUI()
    {
        Debug.Log(backgroundImageBlack.gameObject.name);
        Debug.Log(backgroundImageLetter.gameObject.name);
        Debug.Log(titleText.gameObject.name);
        Debug.Log(textBlockText.gameObject.name);
        Debug.Log(pageCountText.gameObject.name);
        Debug.Log(previousTextBlockBtn.gameObject.name);
        
        backgroundImageBlack.gameObject.SetActive(true);
        backgroundImageLetter.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        textBlockText.gameObject.SetActive(true);
        pageCountText.gameObject.SetActive(true);
        previousTextBlockBtn.gameObject.SetActive(true);
        _globalSfxPlayer.PlaySfx(_globalSfxPlayer.fileReaderPageChange);
    }

    public void DisableFileReaderUI()
    {
        backgroundImageBlack.gameObject.SetActive(false);
        backgroundImageLetter.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        textBlockText.gameObject.SetActive(false);
        pageCountText.gameObject.SetActive(false);
        nextTextBlockBtn.gameObject.SetActive(false);
        closeFileReaderBtn.gameObject.SetActive(false);
        previousTextBlockBtn.gameObject.SetActive(false);
    }
}

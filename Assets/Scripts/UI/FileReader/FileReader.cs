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
    
    void Start()
    {
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
    }

    private void OnCloseFileReaderPressed()
    {
        gameObject.SetActive(false);
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

    private void OnEnable()
    {
        backgroundImageLetter.enabled = true;
        backgroundImageBlack.gameObject.SetActive(true);
        // Enable cursor and pause game should happen on GameManager not here
    }

    private void OnDisable()
    {
        backgroundImageLetter.enabled = false;
        backgroundImageBlack.gameObject.SetActive(false);
        // Disable cursor and pause game should happen on GameManager not here
        if (!GameManager.instance.IsInventoryOpen())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1.0f;
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileReader : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text textBlockText;
    [SerializeField] private TMP_Text pageCountText;
    [SerializeField] private Button nextTextBlockBtn;
    [SerializeField] private Button closeFileReaderBtn;
    [SerializeField] private Button previousTextBlockBtn;
    [SerializeField] private int maxCharactersOnPage = 450;

    private int _pageCount;
    private int _currentPage;
    private string[] _pageData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            nextTextBlockBtn.enabled = false;
            closeFileReaderBtn.enabled = true;
        }
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
            previousTextBlockBtn.enabled = false;
        }
    }

    private void UpdateTextBlock()
    {
        textBlockText.text = _pageData[_currentPage - 1];
    }

    private void UpdatePageCount()
    {
        pageCountText.text = $"{_currentPage}/{_pageCount}";
    }

    public void SetupPageData(string message)
    {
        if (message.Length <= 150)
        {
            _pageCount = 1;
            nextTextBlockBtn.enabled = false;
            closeFileReaderBtn.enabled = true;
            _pageData[0] = message;
        }
        else
        {
            int pageIndex = 0;
            for (int i = 0; i <= message.Length - 1; i += maxCharactersOnPage)
            {
                _pageData[pageIndex] = message.Substring(i, i + maxCharactersOnPage);
                pageIndex++;
            }
            Debug.Log(_pageData.Length);
            _pageCount = _pageData.Length;
            nextTextBlockBtn.enabled = true;
            closeFileReaderBtn.enabled = false;
        }
        
        previousTextBlockBtn.enabled = false;
        _currentPage = 1;
        
        UpdateTextBlock();
        UpdatePageCount();
    }

    private void OnEnable()
    {
        backgroundImage.enabled = true;
        // Enable cursor and pause game (safe to reset these if coming from inventory)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        backgroundImage.enabled = false;
        if (!GameManager.instance.IsInventoryOpen())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1.0f;
        }
    }
}

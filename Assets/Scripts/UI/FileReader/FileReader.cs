using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileReader : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text textBlockText;
    [SerializeField] private TMP_Text pageCountText;
    [SerializeField] private Button nextTextBlock;
    [SerializeField] private Button previousTextBlock;

    private int _pageCount;
    private int _currentPage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextTextBlock.onClick.AddListener(OnNextTextBlockPressed);
        previousTextBlock.onClick.AddListener(OnPreviousTextBlockPressed);
    }

    private void OnNextTextBlockPressed()
    {
        UpdatePageCount();
    }
    
    private void OnPreviousTextBlockPressed()
    {
        UpdatePageCount();
    }

    private void UpdatePageCount()
    {
        pageCountText.text = $"{_currentPage}/{_pageCount}";
    }

    private void OnEnable()
    {
        // Enable cursor and pause game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        
        _currentPage = 0;
    }
}

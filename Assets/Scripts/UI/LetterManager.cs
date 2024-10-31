using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterManager : MonoBehaviour
{
    [SerializeField] private Button _letterExitBtn;
    [SerializeField] private Button _letterContinueBtn;
    [SerializeField] private TMP_Text _textMeshPro;

    [Header("Leave empty if you want to use the text in the TMP Object inside LetterCanvas")]
    [SerializeField] private string _letterContents;
    
    void Start()
    {
        _letterExitBtn.onClick.AddListener(OnLetterExitClicked);
        _letterContinueBtn.onClick.AddListener(OnLetterContinueClicked);
        
        if (_letterContents.Length != 0)
        {
            _textMeshPro.SetText(_letterContents);
        }
    }

    private void OnLetterExitClicked()
    {
        GameManager.instance.ShowMainMenu();
    }

    private void OnLetterContinueClicked()
    {
        // call game manager to deactivate this 
        GameManager.instance.LetterContinue();
    }

    public void SetExitButtonFalse()
    {
        _letterExitBtn.gameObject.SetActive(false);
    }

    public void SetLetterContents(string newContents)
    {
        _textMeshPro.SetText(newContents);
    }
}

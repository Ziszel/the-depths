using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text versionText;
    
    private Button _startGameBtn;
    private Button _optionsBtn;
    private Button _creditsBtn;

    // edit
    private void Awake()
    {
        SetButtonReferences();
        _startGameBtn.onClick.AddListener(OnStartGameClicked);
        _optionsBtn.onClick.AddListener(OnOptionsClicked);
        _creditsBtn.onClick.AddListener(OnCreditsClicked);
    }

    private void Start()
    {
        versionText.text = "Version: " + GameManager.Instance.GetVersionText();
    }

    private void OnStartGameClicked()
    {
        GameManager.Instance.LoadLevel("Level1"); //CHANGE BACK TO MainLevel ONCE TESTING COMPLETE
    }

    private void OnOptionsClicked()
    {
        UIManager.instance.ShowOptionsCanvas(true);
    }
    private void OnCreditsClicked()
    {
        UIManager.instance.ShowCreditsCanvas();
    }
    private void OnCreditsToMainMenuClicked()
    {
        Debug.Log("OPTIONS MENU MAINMENU BUTTON CLICKED");
        UIManager.instance.ShowMainMenu();
    }

    private void SetButtonReferences()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (var b in buttons)
        {
            if (b.gameObject.name == "StartGameBtn")
            {
                _startGameBtn = b;
                continue;
            }

            if (b.gameObject.name == "OptionsBtn")
            {
                _optionsBtn = b;
            }

            if (b.gameObject.name == "CreditsBtn")
            {
                _creditsBtn = b;
            }
        }
    }
}

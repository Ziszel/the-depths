using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private string levelToLoad; // For debug purposes (Level1 on production builds!)
    
    [SerializeField] private GameObject mainMenuUIObj;
    [SerializeField] private GameObject optionsUIObj;
    [SerializeField] private GameObject creditsUIObj;
    
    private GlobalSFXPlayer _globalSFXPlayer;
    
    // MainMenu
    private Button _startGameBtn;
    private Button _optionsBtn;
    private Button _creditsBtn;
    
    // Child scripts
    private SettingsManager _settingsManager;
    
    // Credits (May require separate script for this eventually)
    private Button _returnToMainMenuBtn;

    // edit
    private void Awake()
    {
        _globalSFXPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        
        SetButtonReferences();
        _startGameBtn.onClick.AddListener(OnStartGameClicked);
        _optionsBtn.onClick.AddListener(OnOptionsClicked);
        _creditsBtn.onClick.AddListener(OnCreditsClicked);
        _returnToMainMenuBtn.onClick.AddListener(OnCreditsToMainMenuClicked);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        versionText.text = "Version: " + GameManager.Instance.GetVersionText();
        SetInitialView();
        _settingsManager = GetComponentInChildren<SettingsManager>(true);
    }

    private void OnStartGameClicked()
    {
        _globalSFXPlayer.PlaySfx(_globalSFXPlayer.menuForward);
        GameManager.Instance.LoadLevel(levelToLoad);
    }

    private void OnOptionsClicked()
    {
        _globalSFXPlayer.PlaySfx(_globalSFXPlayer.menuForward);
        _settingsManager.InitialiseSettings();
        optionsUIObj.SetActive(true);
        mainMenuUIObj.SetActive(false);
    }
    private void OnCreditsClicked()
    {
        _globalSFXPlayer.PlaySfx(_globalSFXPlayer.menuForward);
        creditsUIObj.SetActive(true);
        mainMenuUIObj.SetActive(false);
    }
    private void OnCreditsToMainMenuClicked()
    {
        _globalSFXPlayer.PlaySfx(_globalSFXPlayer.menuBackward);
        creditsUIObj.SetActive(false);
        mainMenuUIObj.SetActive(true);
    }

    private void SetInitialView()
    {
        mainMenuUIObj.SetActive(true);
        optionsUIObj.SetActive(false);
        creditsUIObj.SetActive(false);
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
                continue;
            }

            if (b.gameObject.name == "CreditsBtn")
            {
                _creditsBtn = b;
                continue;
            }

            if (b.gameObject.name == "CreditsToMainMenuBtn")
            {
                _returnToMainMenuBtn = b;
            }
        }
    }
}

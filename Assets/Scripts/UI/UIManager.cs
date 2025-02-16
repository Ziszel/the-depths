using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    /* Primary UIs */
    private GameObject _mainMenuCanvas;
    private GameObject _optionsMenuCanvas;
    private GameObject _creditsCanvas;

    /* UI Buttons */
    private GameObject _mainMenuBtn;
    private GameObject _resumeBtn;

    /* Audio */
    private MusicManager _musicManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoadedForUIManager;

        _optionsMenuCanvas = GameObject.Find("OptionsMenuCanvas");
        _mainMenuBtn = _optionsMenuCanvas.transform.Find("OptionsMainMenuBtn").gameObject;
        _resumeBtn = _optionsMenuCanvas.transform.Find("ResumeBtn").gameObject;
    }

    private void OnSceneLoadedForUIManager(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "EndGame")
        {
            _optionsMenuCanvas = GameObject.Find("OptionsMenuCanvas");
            _mainMenuBtn = _optionsMenuCanvas.transform.Find("OptionsMainMenuBtn").gameObject;
            _resumeBtn = _optionsMenuCanvas.transform.Find("ResumeBtn").gameObject;
            _optionsMenuCanvas.SetActive(false);
        }

        if (scene.name == "MainMenu")
        {
            _mainMenuCanvas = GameObject.Find("MainMenuCanvas");
            _mainMenuCanvas.SetActive(true);
            _creditsCanvas = GameObject.Find("CreditsCanvas");
            _creditsCanvas.SetActive(false);

            // Adjust button layout of options menu for main menu
            RectTransform rectTransform = _mainMenuBtn.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(0, -61, 0);
            _resumeBtn.SetActive(false);
        }
        else if (scene.name == "EndGame")
        {
            // don't do anything?
        }
        else // We're in a game level or testing level
        {
            _musicManager = GameObject.Find("MusicAudioSource").GetComponentInChildren<MusicManager>();

            // Adjust options menu layout for game level
            RectTransform rectTransform = _mainMenuBtn.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(146, -323, 0);
            _resumeBtn.SetActive(true);
        }
    }

    public void ShowOptionsCanvas(bool isFromMainMenu)
    {
        if (_optionsMenuCanvas)
        {
            _optionsMenuCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("GameManager ShowOptionsCanvas(): Could not find the options menu canvas object");
        }
        if (_mainMenuCanvas)
        {
            _mainMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("GameManager ShowOptionsCanvas(): Could not find the main menu canvas object");
        }
    }

    public void UnshowOptionsCanvas()
    {
        _optionsMenuCanvas.SetActive(false);
    }

    // Maybe rename this function - backto main menu from options or something
    public void ShowMainMenu()
    {
        if (_mainMenuCanvas)
        {
            // we know we are in the MainMenu level so we just need to activate the canvas
            _mainMenuCanvas.SetActive(true);
        }
        else
        {
            // This must mean we are in-game if the main menu canvas does not exist so we want to fully return to the main menu level
            GameManager.Instance.LoadLevel("MainMenu");
        }
        if (_optionsMenuCanvas)
        {
            _optionsMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("GameManager ShowMainMenuCanvas(): Could not find the options menu canvas object");
        }
        if (_creditsCanvas)
        {
            _creditsCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("GameManager ShowMainMenuCanvas(): Could not find the options menu canvas object");
        }
    }

    // we know this will only ever get called from the main menu
    public void ShowCreditsCanvas()
    {
        // Show options menu overlay (should work on MainMenu and in game)
        Debug.Log("ShowOptionsCanvas() called");

        if (_creditsCanvas)
        {
            _creditsCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("GameManager ShowOptionsCanvas(): Could not find the credits canvas object");
        }
        if (_mainMenuCanvas)
        {
            _mainMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("GameManager ShowOptionsCanvas(): Could not find the main menu canvas object");
        }
    }

}

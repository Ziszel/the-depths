using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
  
    private float _bestTime;
    private int _letterCount;

    /* Primary UIs */
    private GameObject _mainMenuCanvas;
    private GameObject _optionsMenuCanvas; 
    private GameObject _creditsCanvas;
    private GameObject _letterCanvas;

    /* UI Buttons */
    private GameObject _mainMenuBtn;
    private GameObject _resumeBtn;

    /* Audio */
    private MusicManager _musicManager;

    /* Monster */
    Monster monster;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _bestTime = 999999;

        SceneManager.sceneLoaded += OnSceneLoaded;

        _optionsMenuCanvas = GameObject.Find("OptionsMenuCanvas");
        _mainMenuBtn = _optionsMenuCanvas.transform.Find("OptionsMainMenuBtn").gameObject;
        _resumeBtn = _optionsMenuCanvas.transform.Find("ResumeBtn").gameObject;
    }

    public void LoadLevel(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "EndGame")
        {
            _optionsMenuCanvas = GameObject.Find("OptionsMenuCanvas");
            _mainMenuBtn = _optionsMenuCanvas.transform.Find("OptionsMainMenuBtn").gameObject;
            _resumeBtn = _optionsMenuCanvas.transform.Find("ResumeBtn").gameObject;
            _optionsMenuCanvas.SetActive(false);
        }
        
        // Cursor will be ALWAYS be shown and not locked at the start
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
            _letterCount = 0;
        }
        else if (scene.name == "EndGame")
        {
            // don't do anything?
        }
        else // We're in a game level or testing level
        {
            _letterCanvas = GameObject.Find("LetterCanvas");
            _musicManager = GameObject.Find("MusicAudioSource").GetComponentInChildren<MusicManager>();
            monster = GameObject.Find("Monster").GetComponentInChildren<Monster>(); // Get the monster stored so we're able to play chasing/wandering music
            if (monster != null)
            {
                monster.OnChaseStateEntered += HandleMonsterEnterChaseState;
                monster.OnChaseStateExited += HandleMonsterExitChaseState;
            }

            // Adjust options menu layout for game level
            RectTransform rectTransform = _mainMenuBtn.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(146, -323, 0);
            _resumeBtn.SetActive(true);

            // First we start game level with a black screen, fading into the letter screen, with music
            ActivateLetter();
        }
    }

    // Ideally, we don't want mainmenu canvas to be a parameter here, as we want this to be able to be called from in-game
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
            LoadLevel("MainMenu");
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

    public void LetterContinue()
    {
        DeactivateLetter();
    }
    
    public void ActivateLetter()
    {
        if (_letterCount != 0)
        {
            if (_letterCanvas.TryGetComponent(out LetterManager lm))
            {
                lm.SetExitButtonFalse();
            }
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        _musicManager.AssignIntroMusic();
        _musicManager.Play();

        _letterCanvas.SetActive(true);
        _letterCount++;
    }

    public void DeactivateLetter()
    {
        // Deactivate the cursor when resuming the game after reading a letter
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure game is unpaused after user reads letter
        Time.timeScale = 1f; 

        _musicManager.TriggerFadeOutMusic();

        _letterCanvas.SetActive(false);
    }

    public bool IsLetterActive()
    {
        return _letterCanvas.activeSelf;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowOptionsCanvas(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _optionsMenuCanvas.SetActive(false);
    }

    private void HandleMonsterEnterChaseState()
    {
        // Need an if here otherwise this will get assigned chase and play every frame from the monster delegate
        if (!_musicManager.IsPlaying())
        {
            _musicManager.AssignChaseMusic();
            _musicManager.Play();
        }
    }

    private void HandleMonsterExitChaseState()
    {
        _musicManager.TriggerFadeOutMusic(1.5f);
    }

    public void SetBestTime(float newBestTime)
    {
        if (newBestTime < _bestTime)
        {
            _bestTime = newBestTime;
        }
    }

    public float GetBestTime()
    {
        return _bestTime;
    }
    
}

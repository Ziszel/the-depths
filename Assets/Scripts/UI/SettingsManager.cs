using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Button returnToMainMenuButton;
    
    // Load values from PlayerPrefs and update UI elements to match
    public void InitialiseSettings()
    {
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
        
        returnToMainMenuButton.onClick.AddListener(OnExitBtnClicked);
    }

    public void OnSfxVolumeChanged()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }

    public void OnMusicVolumeChanged()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    public void OnMouseSensitivityChanged()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
    }

    private void OnExitBtnClicked()
    {
        // Update this to give the player a warning first (also asking if they want to cancel changes)
        PlayerPrefs.Save(); // These will be loaded next time the game is loaded so no LevelManager required
        GameManager.Instance.LoadLevel("MainMenu");
    }
}

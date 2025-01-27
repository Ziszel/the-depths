using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameLevelManager : MonoBehaviour
{
    // Store references to elements we will change
    public Button returnToMenuBtn;
    public TMP_Text completionTimeValue;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Not needed as handled in GameManager, but better practice without a proper UI manager
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        
        SetCompletionTimeValue();
        returnToMenuBtn.onClick.AddListener(OnReturnBtnClicked);
    }

    public void SetCompletionTimeValue()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.instance.GetBestTime());
        completionTimeValue.text = timeSpan.ToString("hh':'mm':'ss", new CultureInfo("en-GB"));
    }

    public void OnReturnBtnClicked()
    {
        GameManager.instance.LoadLevel("MainMenu");
    }
}

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
    
    void Start()
    {
        SetCompletionTimeValue();
        returnToMenuBtn.onClick.AddListener(OnReturnBtnClicked);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void SetCompletionTimeValue()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.GetBestTime());
        completionTimeValue.text = timeSpan.ToString("hh':'mm':'ss", new CultureInfo("en-GB"));
    }

    public void OnReturnBtnClicked()
    {
        GameManager.Instance.LoadLevel("MainMenu");
    }
}

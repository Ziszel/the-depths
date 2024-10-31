using UnityEngine;
using UnityEngine.UI;
public class CreditsManager : MonoBehaviour
{
    // edit
    private Button _creditsMainMenuBtn;

    void Awake()
    {
        _creditsMainMenuBtn = transform.Find("CreditsMainMenuBtn").GetComponent<Button>();
        _creditsMainMenuBtn.onClick.AddListener(OnCreditsToMainMenuClicked);
    }
    private void OnCreditsToMainMenuClicked()
    {
        GameManager.instance.ShowMainMenu();
    }

}

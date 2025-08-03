using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackFadeTransition : MonoBehaviour
{
    public static event Action OnFadeInEventComplete;
    public static event Action OnFadeOutEventComplete;
    
    public Image blackBackground;
    
    private const float DefaultFadeTransition = 1.0f;
    
    public IEnumerator FadeToBlack(float transitionDuration = DefaultFadeTransition)
    {
        blackBackground.gameObject.SetActive(true);
        float elapsedTime = 0.0f;
        SetBackgroundTransparency(0);
        // yield return new WaitForSeconds(1f);

        while (elapsedTime < transitionDuration)
        {
            SetBackgroundTransparency(Mathf.Clamp01(elapsedTime / transitionDuration));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        SetBackgroundTransparency(1);
        OnFadeInEventComplete?.Invoke();
    }
    
    public IEnumerator FadeFromBlack(float transitionDuration = DefaultFadeTransition)
    {
        float elapsedTime = 0;
        SetBackgroundTransparency(1);

        while (elapsedTime < transitionDuration)
        {
            SetBackgroundTransparency(Mathf.Clamp01(1 - (elapsedTime / transitionDuration)));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        SetBackgroundTransparency(0);
        blackBackground.gameObject.SetActive(false);
        OnFadeOutEventComplete?.Invoke();
    }

    private void SetBackgroundTransparency(float transparency)
    {
        Color newColour = blackBackground.color;
        newColour.a = transparency;
        blackBackground.color = newColour;
    }
}

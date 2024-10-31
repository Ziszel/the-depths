using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class BlackFadeTransition : MonoBehaviour
{
    private Image blackBackground;
    private float transitionDuration = 2.0f;

    private void Awake()
    {
        blackBackground = transform.Find("BlackBackground").GetComponent<Image>();
    }

    public void TriggerFadeToBlack(Action onFadeComplete)
    {
        StartCoroutine(FadeToBlack(onFadeComplete));
    }

    public void TriggerFadeFromBlack(Action onFadeComplete)
    {
        StartCoroutine(FadeFromBlack(onFadeComplete));
    }

    public void TriggerFadeToBlackAndBack(Action onFadeToBlack, Action onFadeBackIn)
    {
        StartCoroutine(FadeToBlackAndBack(onFadeToBlack, onFadeBackIn));
    }
    
    public IEnumerator CalinFadeToBlack()
    {
        float elapsedTime = 0;
        SetBackgroundTransparency(0);
        // yield return new WaitForSeconds(1f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            SetBackgroundTransparency(Mathf.Clamp01(elapsedTime / transitionDuration));
            //Debug.Log("FadeToBlack called. elapsed time: " + elapsedTime);
            yield return null;
        }
        
        GameManager.instance.DeactivateLetter();
        StartCoroutine(CalinFadeFromBlack());
    }
    
    public IEnumerator CalinFadeFromBlack()
    {
        float elapsedTime = 0;
        SetBackgroundTransparency(1);
        // yield return new WaitForSecondsRealtime(0.5f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            SetBackgroundTransparency(Mathf.Clamp01(1 - (elapsedTime / transitionDuration)));
            //Debug.Log("FadeFromBlack called. elapsed time: " + elapsedTime);
            yield return null;
        }
        
    }

    private IEnumerator FadeToBlack(Action onFadeComplete)
    {
        float elapsedTime = 0;
        SetBackgroundTransparency(0);
        yield return new WaitForSeconds(1f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            SetBackgroundTransparency(Mathf.Clamp01(elapsedTime / transitionDuration));
            //Debug.Log("FadeToBlack called. elapsed time: " + elapsedTime);
            yield return null;
        }
        
        onFadeComplete?.Invoke();
    }

    private IEnumerator FadeFromBlack(Action onFadeComplete)
    {
        float elapsedTime = 0;
        SetBackgroundTransparency(1);
        yield return new WaitForSecondsRealtime(0.5f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            SetBackgroundTransparency(Mathf.Clamp01(1 - (elapsedTime / transitionDuration)));
            //Debug.Log("FadeFromBlack called. elapsed time: " + elapsedTime);
            yield return null;
        }
        
        onFadeComplete?.Invoke();
    }

    private IEnumerator FadeToBlackAndBack(Action onFadeToBlack, Action onFadeBackIn)
    {
        float elapsedTime = 0;
        SetBackgroundTransparency(0);
        yield return new WaitForSeconds(1f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            SetBackgroundTransparency(Mathf.Clamp01(elapsedTime / transitionDuration));
            //Debug.Log("FadeToBlack called. elapsed time: " + elapsedTime);
            yield return null;
        }

        onFadeToBlack?.Invoke();
        SetBackgroundTransparency(1);
        yield return new WaitForSecondsRealtime(1f); // Wait for a moment before fading back in

        // Now fade back in
        elapsedTime = 0;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            SetBackgroundTransparency(Mathf.Clamp01(1 - (elapsedTime / transitionDuration)));
            yield return null;
        }

        SetBackgroundTransparency(0); // Ensure it's fully transparent
        
        onFadeBackIn?.Invoke();
    }

    private void SetBackgroundTransparency(float transparency)
    {
        Color newColour = blackBackground.color;
        newColour.a = transparency;
        blackBackground.color = newColour;
    }
}

using System;
using UnityEngine;

public class DisplayDescriptiveText : MonoBehaviour
{
    public static event Action<string> OnTextUpdate;
    
    [SerializeField] private string descriptionText;

    public void UpdateDescriptionText()
    {
        OnTextUpdate?.Invoke(descriptionText);
    }

    public void UpdateDescriptionText(string text)
    {
        descriptionText = text;
        OnTextUpdate?.Invoke(descriptionText);
    }

    public void UpdateDescriptionTextLocally(string text)
    {
        descriptionText = text;
    }
}

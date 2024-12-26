using System;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public static event Action<Vector3, float> OnSoundTriggered;

    [Header("Trigger settings")] [SerializeField]
    private float triggerVolume = 10.0f; // in unity units (meters)

    // Where the sound occurs and where the monster could go.
    // We might not ALWAYS want it to happen at the trigger source (switch opening a door for example)
    [SerializeField] private Vector3 triggerLocation;
    [SerializeField] private bool overwriteTransform;
    
    private void Start()
    {
        if (!overwriteTransform)
        {
            triggerLocation = GetComponentInParent<Transform>().position;
        }
    }

    public void TriggerSound()
    {
        OnSoundTriggered?.Invoke(triggerLocation, triggerVolume);
    }
}

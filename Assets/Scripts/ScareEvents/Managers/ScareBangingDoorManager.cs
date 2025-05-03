using System.Collections;
using UnityEngine;

// Scare Event Manager classes are bespoke and used to control the order of execution of scare events
// if playing multiple at the same time the player enters the trigger is not desirable.
public class ScareBangingDoorManager : MonoBehaviour, IScareEvent
{
    [SerializeField] private AudioClip monsterScreech;
    [SerializeField] private AudioClip chaseMusic;
    [SerializeField] private float phaseTwoDelayTime;
    private MusicManager _musicManager;
    private GlobalSFXPlayer _sfxPlayer;
    private Monster _monster;
    
    // Components | NOTE Unity does not support exposing interfaces in the inspector so I am using these as
    // classes since I know how they work. This isn't ideal but will work for my use-case for now
    private ScarePush _scarePush;
    private ScareRotateByTorque _scareRotateByTorque;

    private void Start()
    {
        _musicManager = FindAnyObjectByType<MusicManager>();
        _sfxPlayer = FindAnyObjectByType<GlobalSFXPlayer>();
        _scarePush = GetComponent<ScarePush>();
        _scareRotateByTorque = GetComponent<ScareRotateByTorque>();
        _monster = FindAnyObjectByType<Monster>();
    }

    private IEnumerator DelayPhaseTwo(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TriggerPhaseTwoScareEvents();
    }

    private void TriggerPhaseOneScareEvents()
    {
        // do nothing
    }

    private void TriggerPhaseTwoScareEvents()
    {
        _scarePush.TriggerScareEvent();
        _scareRotateByTorque.TriggerScareEvent();
    }
    
    private IEnumerator DelaySFX(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _sfxPlayer.PlaySfx(monsterScreech);
    }

    public void TriggerScareEvent()
    {
        _musicManager.PlayMusic(chaseMusic, 0.5f);
        StartCoroutine(DelaySFX(2.0f));
        TriggerPhaseOneScareEvents();
        StartCoroutine(DelayPhaseTwo(phaseTwoDelayTime));
    }
}

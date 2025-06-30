using System.Collections;
using System.Linq;
using UnityEngine;

// Scare Event Manager classes are bespoke and used to control the order of execution of scare events
// if playing multiple at the same time the player enters the trigger is not desirable.
public class ScareBangingDoorManager : MonoBehaviour, IScareEvent, IResettable
{
    [SerializeField] private AudioClip monsterScreech;
    [SerializeField] private AudioClip chaseMusic;
    [SerializeField] private float phaseTwoDelayTime;
    [SerializeField] private Vector3[] initialMonsterPathNodes;
    [SerializeField] private Vector3 initialInvestigationPosition;
    private MusicManager _musicManager;
    private GlobalSFXPlayer _sfxPlayer;
    private Monster _monster;
    private FlyingDoor _flyingDoor;
    
    // Components | NOTE Unity does not support exposing interfaces in the inspector so I am using these as
    // classes since I know how they work. This isn't ideal but will work for my use-case for now
    private ScarePush _scarePush;
    private ScareRotateByTorque _scareRotateByTorque;
    private ScareShake _scareShake;

    private void Start()
    {
        _musicManager = FindAnyObjectByType<MusicManager>();
        _sfxPlayer = FindAnyObjectByType<GlobalSFXPlayer>();
        _scarePush = GetComponent<ScarePush>();
        _scareRotateByTorque = GetComponent<ScareRotateByTorque>();
        _scareShake = GetComponent<ScareShake>();
        _monster = FindAnyObjectByType<Monster>();
        _flyingDoor = GetComponent<FlyingDoor>();
    }

    private IEnumerator DelayPhaseTwo(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TriggerPhaseTwoScareEvents();
    }

    private void TriggerPhaseOneScareEvents()
    {
        _scareShake.enabled = true; // activate update loop
        _scareShake.TriggerScareEvent();
    }

    private void TriggerPhaseTwoScareEvents()
    {
        _scareShake.enabled = false;
        _scarePush.TriggerScareEvent();
        _scareRotateByTorque.TriggerScareEvent();
        _musicManager.StopMusicWithDelay(2.0f);
        _monster.SetMonsterState(Monster.MonsterState.Investigate, initialMonsterPathNodes.ToList(), 
            _monster.transform.position, initialInvestigationPosition);
        _flyingDoor.SetFlyingDoor(true);
        _flyingDoor.SetWalkableTag();
    }
    
    private IEnumerator DelaySFX(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _sfxPlayer.PlaySfx(monsterScreech);
    }

    public void TriggerScareEvent()
    {
        _musicManager.PlayMusic(chaseMusic, 0.5f);
        StartCoroutine(DelaySFX(2.0f)); // Play monster screech after music has started
        TriggerPhaseOneScareEvents();
        StartCoroutine(DelayPhaseTwo(phaseTwoDelayTime));
    }

    public void ResetObjectState()
    {
        _scareShake.SetInitialState();
        _flyingDoor.SetInitialState();
        _scarePush.SetInitialState();
    }
}

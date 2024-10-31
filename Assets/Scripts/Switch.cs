using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private float switchMovementTime = 2.0f;
    private Vector3 rotationVectorUp = new ( 0.0f, 0.0f, 60.0f );
    private Vector3 rotationVectorDown = new (0.0f, 0.0f, 125.0f);

    private Animator _animator;
    public Action<GameObject> SwitchAnimation;
    
    public List<GameObject> Switchables;
    private PlayerController _player;
    private bool _isSwitchDown;
    private Transform _lever;
    private SwitchAudio _switchAudio;
    private LevelManager _levelManager;
    
    private void Start()
    {
        _isSwitchDown = false;
        _player = FindAnyObjectByType<PlayerController>();
        _lever = GetComponentsInChildren<Transform>().First(k => k.gameObject.name == "Lever");
        _switchAudio = GetComponent<SwitchAudio>();
        _levelManager = FindAnyObjectByType<LevelManager>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _levelManager.ActivateInteractUI();
            _player.SetCurrentInteractable(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _levelManager.DeActivateInteractUI();
            _player.SetCurrentInteractable(null);
        }
    }

    private IEnumerator MoveSwitch()
    {
        Vector3 moveToRotation;
        if (!_isSwitchDown)
        {
            moveToRotation = rotationVectorDown;
        }
        else
        {
            moveToRotation = rotationVectorUp;
        }
        
        float timeElapsed = 0.0f;

        /*while (timeElapsed < switchMovementTime)
        {
            if (Vector3.Distance(_lever.eulerAngles, moveToRotation) > 0.01f)
            {
                _lever.eulerAngles = Vector3.Lerp(_lever.rotation.eulerAngles, moveToRotation, timeElapsed / switchMovementTime);
            }
            yield return null;
        }*/

        _lever.localEulerAngles = moveToRotation;

        int i = _isSwitchDown ? 0 : 1;

        if (i == 0) { _isSwitchDown = false; }
        else { _isSwitchDown = true; }

        _lever.eulerAngles = moveToRotation;
        
        yield return null;
    }

    public void AttemptToInteract()
    {
        foreach (var Switchable in Switchables)
        {
            if (Switchable.TryGetComponent(out ISwitchable switchable))
            {
                Debug.Log(_animator.name);
                _animator.SetBool("Pressed", true);
                //StartCoroutine(MoveSwitch());
                SwitchAnimation?.Invoke(this.gameObject);
                switchable.Toggle();
                _switchAudio.PlaySfx();
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class SetMonsterStateTrigger : MonoBehaviour
{
    [SerializeField] private Monster.MonsterState _monsterState;
    [SerializeField] private Vector3 monsterPosition;
    [SerializeField] private List<Vector3> pathNodes;

    private BoxCollider _collider;
    
    private Monster _monster;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _monster = FindAnyObjectByType<Monster>();
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_monsterState == Monster.MonsterState.None)
            {
                // will handle moving to none properly
                _monster.SetMonsterState(_monsterState);
            }
            else
            {
                // paths and chase need the alternative SetMonsterState call
                _monster.SetMonsterState(_monsterState, pathNodes, monsterPosition);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _collider.enabled = false;
        }
    }

    public void SetColliderOn()
    {
        _collider.enabled = true;
    }
}

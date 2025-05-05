using System.Collections.Generic;
using UnityEngine;
/* NOTE: Old game jam class which did not support investigate monster state, will likely need updating if ever reused.
 
 Candidate for deprecation! Consider complete re-write or removal!
 */
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
                // NOTE: Old class which did not support investigate, will likely need updating if ever reused.
                _monster.SetMonsterState(_monsterState, pathNodes, monsterPosition,
                    Vector3.zero);
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

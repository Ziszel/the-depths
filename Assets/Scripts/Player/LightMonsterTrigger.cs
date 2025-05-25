using System;
using UnityEngine;

public class LightMonsterTrigger : MonoBehaviour
{
    public static Action OnLightHitMonster;

    [SerializeField] private float fieldOfViewAngle;
    [SerializeField] private float lightRayRange;
    
    private Light _flashlightSource;
    private Monster _monster;
    private LayerMask _layerMask;

    void Start()
    {
        _flashlightSource = GetComponentInChildren<Light>(true);
        _monster = FindAnyObjectByType<Monster>();
        _layerMask = LayerMask.GetMask("Player");
        _layerMask = ~_layerMask;
    }

    private void Update()
    {
        if (_flashlightSource.isActiveAndEnabled)
        {
            if (HasLightHitMonster())
            {
                // If monster is already chasing do not set state to chasing again
                if (_monster.GetMonsterState() != Monster.MonsterState.Chase)
                {
                    Debug.Log("Monster not chasing, setting state to chase!");
                    OnLightHitMonster.Invoke();
                }
            }
        }
    }

    private bool HasLightHitMonster()
    {
        Vector3 directionOfRay = (_monster.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, directionOfRay);
        // Debug.DrawRay(transform.position, directionOfRay * lightRayRange, Color.red);
        
        if (Vector3.Angle(directionOfRay, transform.forward) < fieldOfViewAngle / 2)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, lightRayRange, _layerMask))
            {
                // Debug.Log(hit.collider.name);
                if (hit.collider.gameObject.CompareTag("Monster"))
                {
                    // Debug.Log("ray hit monster");
                    return true;
                }
            }
        }
        return false;
    }
}

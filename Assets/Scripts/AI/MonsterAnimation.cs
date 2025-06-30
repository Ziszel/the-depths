using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetStateToWalk()
    {
        // TODO: Run will always be active currently. Update later
        //_animator.SetBool("Sprint", false);
    }
    
    public void SetStateToSprint()
    {
        // TODO: Run will always be active currently. Update later
        //_animator.SetBool("Sprint", true);
    }
}

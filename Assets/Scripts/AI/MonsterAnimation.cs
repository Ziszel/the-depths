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
        _animator.SetBool("Sprint", false);
    }
    
    public void SetStateToSprint()
    {
        _animator.SetBool("Sprint", true);
    }
}

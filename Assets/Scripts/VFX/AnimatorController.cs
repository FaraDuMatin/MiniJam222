using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public void Grounded(bool grounded)
    {
        _animator.SetBool("IsGrounded", grounded);
    }
    public void Jumped()
    {
        _animator.SetTrigger("Jumped");
    }

    public void GrabWall(float direction)
    {
        _animator.SetTrigger("GrabWall");
        _animator.transform.localScale = new Vector3(direction, 1, 1);
    }

    public void Move(float direction)
    {
        if(direction > 0)
            direction = 1;
        if(direction < 0)
            direction = -1;
        
        _animator.SetBool("Running", direction != 0);
        if(direction != 0)
            _animator.transform.localScale = new Vector3(direction, 1, 1);
    }
}

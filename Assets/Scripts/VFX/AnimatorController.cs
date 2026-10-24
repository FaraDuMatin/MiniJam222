using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] Animator[] _animator;

    public void Grounded(bool grounded)
    {
        foreach(var animator in _animator)
            animator.SetBool("IsGrounded", grounded);
    }
    public void Jumped()
    {
        foreach(var animator in _animator)
            animator.SetTrigger("Jumped");
    }

    public void IsJumping(bool jumping)
    {
        foreach(var animator in _animator)
            animator.SetBool("IsJumping", jumping);
    }

    public void GrabWall(float direction)
    {
        foreach(var animator in _animator)
        {
            animator.SetTrigger("GrabWall");
            animator.transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    public void Move(float direction)
    {
        if(direction > 0)
            direction = 1;
        if(direction < 0)
            direction = -1;
        
        foreach(var animator in _animator)
        {
            animator.SetBool("Running", direction != 0);
            if(direction != 0)
                animator.transform.localScale = new Vector3(direction, 1, 1);
        }
    }
}

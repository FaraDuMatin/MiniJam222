using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class PlayerCharacter : MonoBehaviour
{



    [SerializeField] Rigidbody2D _rigidbody2D;
    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] float _moveSpeed = 10f;
    private Vector2 _moveInput;




    [Header("Jump Settings")]
    [SerializeField] float _jumpHeight = 2f;
    [SerializeField] float _timeToApex = 0.5f;
    float _jumpVelocity;

    bool _isJumping;

    float _jumpBoostMultiplier = 1f;
    float _jumpBoostTimer;



    [Header("Gravity Settings")]

    float _normalGravity = 9.81f;
    [SerializeField] float _fallingGravity = 12f;
    [SerializeField] float _snapToGround = 1f;


    [Header("Ground Check Settings")]
    [SerializeField] Vector2 _groundCheckOffset = new Vector2(0f, -0.5f);
    [SerializeField] float _groundCheckRadius = 0.2f;
    [SerializeField] LayerMask _groundLayer;

    bool _isGrounded;


    [Header("Wall Check Settings")]
    [SerializeField] Vector2 _wallCheckOffset = new Vector2(0f, 1f);
    [SerializeField] float _wallCheckDistance = 0.6f;
    [SerializeField] float _wallCheckSpacing = 0.4f;
    [SerializeField] LayerMask _obstacleLayer;

    bool _isWallRight;
    bool _isWallLeft;


    [Header("Ability Settings")]
    [SerializeField] private List<AbilitySO> _abilities = new List<AbilitySO>();
    bool _isEnchanting = false;
    List<AbilityKey> _currentAbilityKeys = new List<AbilityKey>();



    [Header("Claws Settings")]
    [SerializeField] float _wallSlideSpeed = 1f;
    [SerializeField] Vector2 _wallJumpVelocity = new Vector2(8f, 12f);
    [SerializeField] float _wallJumpLockTime = 0.2f;

    bool _canGrip;
    bool _isGripping;
    float _gripTimer;
    float _wallJumpLockTimer;


    void Start()
    {
        _rigidbody2D.gravityScale = 0f; // Disable default gravity
        _normalGravity = (2f * _jumpHeight) / (_timeToApex * _timeToApex);
        _jumpVelocity = (2f * _jumpHeight) / _timeToApex;

    }




    void Update()
    {
        // Feline Agility Timer
        if (_jumpBoostTimer > 0)
        {
            _jumpBoostTimer -= Time.deltaTime;
            if (_jumpBoostTimer <= 0)
            {
                _jumpBoostMultiplier = 1f;
            }
        }

        // Claws Timer
        if (_gripTimer > 0)
        {
            _gripTimer -= Time.deltaTime;
            if (_gripTimer <= 0)
            {
                _canGrip = false;
            }
        }

        if (_wallJumpLockTimer > 0)
            _wallJumpLockTimer -= Time.deltaTime;

        _isGripping = _canGrip && !_isGrounded && _rigidbody2D.linearVelocity.y <= 0
            && ((_isWallRight && _moveInput.x > 0) || (_isWallLeft && _moveInput.x < 0));


        // Base Behavior Checks
        bool lastGroundedState = _isGrounded;
        _isGrounded = CheckGrounded();

        _isWallRight = CheckWall(Vector2.right);
        _isWallLeft = CheckWall(Vector2.left);

        if (_isJumping && _isGrounded)
        {
            // _isGrounded = false;

            if (_rigidbody2D.linearVelocity.y <= 0)
            {
                _isJumping = false;
            }
        }

        if (_isJumping && _isGrounded && !lastGroundedState)
        {
            _isJumping = false;
        }

        UpdateColor();
    }

    void FixedUpdate()
    {
        HandleGravity();
        HandleMovement();
    }


    void HandleMovement()
    {
        // Movement locked while gripping. !!! NOT SURE TODO !!! (check later)
        if (_wallJumpLockTimer > 0) return;


        Vector2 velocity = _rigidbody2D.linearVelocity;
        velocity.x = _moveInput.x * _moveSpeed;
        // if _isWallRight && _moveInput.x > 0, then velocity.x = 0
        // pareil pour wall left
        // en gros il se cogne
        if ((_isWallRight && velocity.x > 0) || (_isWallLeft && velocity.x < 0))
        {
            velocity.x = 0;
        }



        _rigidbody2D.linearVelocity = velocity;
    }
    void HandleGravity()
    {

        // Claws Grip Logic
        if (_isGripping)
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, -_wallSlideSpeed);
            return;
        }


        if (_isGrounded && !_isJumping)
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, -_snapToGround);
        }


        if (_rigidbody2D.linearVelocity.y < 0)
        {
            _rigidbody2D.AddForce(Vector2.down * _fallingGravity * _rigidbody2D.mass);
        }
        else
        {
            _rigidbody2D.AddForce(Vector2.down * _normalGravity * _rigidbody2D.mass);
        }


    }


    public void MoveInput(Vector2 moveInput)
    {
        // Handle movement logic here
        _moveInput = moveInput;

    }

    void Jump()
    {

        _isJumping = true;
        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpVelocity * _jumpBoostMultiplier);


    }

    void WallJump()
    {
        float direction = _isWallRight ? -1f : 1f;
        _isJumping = true;
        _isGripping = false;
        _wallJumpLockTimer = _wallJumpLockTime;
        _rigidbody2D.linearVelocity = new Vector2(direction * _wallJumpVelocity.x, _wallJumpVelocity.y);
    }


    public bool TryJump()
    {
        if (_canGrip && !_isGrounded && (_isWallRight || _isWallLeft))
        {
            WallJump();
            return true;
        }


        if (_isGrounded && !_isJumping)
        {
            Jump();
            return true;
        }
        return false;


    }


    bool CheckGrounded()
    {
        Vector2 position = (Vector2)transform.position + _groundCheckOffset;
        return Physics2D.OverlapCircle(position, _groundCheckRadius, _groundLayer);
    }

    bool CheckWall(Vector2 direction)
    {
        for (int i = -1; i <= 1; i++)
        {
            Vector2 origin = (Vector2)transform.position + _wallCheckOffset + Vector2.up * (i * _wallCheckSpacing);
            if (Physics2D.Raycast(origin, direction, _wallCheckDistance, _obstacleLayer))
                return true;
        }
        return false;
    }




    // Ability System 
    #region Ability System

    AbilitySO SearchAbility(List<AbilityKey> abilityKeys)
    {
        foreach (var ability in _abilities)
        {
            // Contains doesn't work because order matters, so we use SequenceEqual
            if (ability.AbilityKeys.SequenceEqual(abilityKeys))
            {
                return ability;
            }
        }

        return null; // No matching ability found
    }


    void HandleAbilityInput(List<AbilityKey> inputKeys)
    {
        Debug.Log("Sequence : " + string.Join(", ", inputKeys));
        AbilitySO ability = SearchAbility(inputKeys);
        if (ability != null)
        {
            // Execute the ability logic here
            ability.ExecuteAbility(this);
        }
    }


    public void ManageAbility(bool value)
    {
        _isEnchanting = value;
        _currentAbilityKeys.Clear();
    }

    public void AddAbilityKey(AbilityKey key)
    {
        if (!_isEnchanting) return;

        _currentAbilityKeys.Add(key);
        if (_currentAbilityKeys.Count >= 3)
        {
            HandleAbilityInput(_currentAbilityKeys);
            _currentAbilityKeys.Clear();
        }
    }


    public void ApplyJumpBoost(float multiplier, float duration)
    {
        _jumpBoostMultiplier = multiplier;
        _jumpBoostTimer = duration;
    }

    public void ApplyClaws(float duration)
    {
        _canGrip = true;
        _gripTimer = duration;
    }


    #endregion Ability System





    void UpdateColor()
    {
        if (_canGrip)
            _spriteRenderer.color = Color.red;
        else if (_jumpBoostTimer > 0)
            _spriteRenderer.color = Color.yellow;
        else
            _spriteRenderer.color = Color.white;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 position = (Vector2)transform.position + _groundCheckOffset;
        Gizmos.DrawWireSphere(position, _groundCheckRadius);

        Gizmos.color = Color.blue;
        for (int i = -1; i <= 1; i++)
        {
            Vector2 origin = (Vector2)transform.position + _wallCheckOffset + Vector2.up * (i * _wallCheckSpacing);
            Gizmos.DrawLine(origin, origin + Vector2.right * _wallCheckDistance);
            Gizmos.DrawLine(origin, origin + Vector2.left * _wallCheckDistance);
        }
    }
}

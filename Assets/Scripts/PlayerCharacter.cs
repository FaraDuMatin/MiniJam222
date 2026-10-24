using UnityEngine;
using System.Collections.Generic;
public class PlayerCharacter : MonoBehaviour
{



    [SerializeField] Rigidbody2D _rigidbody2D;
    [SerializeField] float _moveSpeed = 10f;
    private Vector2 _moveInput;




    [Header("Jump Settings")]
    [SerializeField] float _jumpHeight = 2f;
    [SerializeField] float _timeToApex = 0.5f;
    float _jumpVelocity;

    bool _isJumping;


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


    void Start()
    {
        _rigidbody2D.gravityScale = 0f; // Disable default gravity
        _normalGravity = (2f * _jumpHeight) / (_timeToApex * _timeToApex);
        _jumpVelocity = (2f * _jumpHeight) / _timeToApex;

    }




    void Update()
    {
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
    }

    void FixedUpdate()
    {
        HandleGravity();
        HandleMovement();
    }


    void HandleMovement()
    {
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
        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpVelocity);

    }

    public bool TryJump()
    {
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
            bool allKeysMatch = true;
            foreach (var key in ability.AbilityKeys)
            {
                if (!abilityKeys.Contains(key))
                {
                    allKeysMatch = false;
                    break;
                }
            }

            if (allKeysMatch)
            {
                return ability;
            }
        }

        return null; // No matching ability found
    }

    void HandleAbilityInput(List<AbilityKey> inputKeys)
    {
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

    #endregion Ability System





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

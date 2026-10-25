using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class PlayerCharacter : MonoBehaviour
{



    [SerializeField] Rigidbody2D _rigidbody2D;

    [Header("VFX Settings")]
    [SerializeField] AnimatorController _animatorController;
    [SerializeField] VFXEnchant _vfxEnchant;
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
    [SerializeField] int _raysPerSide = 3;
    [SerializeField] Vector2 _catWallCheckOffset = new Vector2(0f, 0.3f);
    [SerializeField] int _catRaysPerSide = 3;


    bool _isWallRight;
    bool _isWallLeft;

    [Header("Interaction Settings")]
    [SerializeField] Vector2 _interactionOffset = new Vector2(0f, 0f);
    [SerializeField] float _interactionRange = 1f;
    [SerializeField] LayerMask _interactableLayer;
    bool _interact = false;
    float _interactionTime = 0f;
    Interactable _hoverInteractable;
    Interactable _currentInteractable;
    [Space]
    [SerializeField] GameObject _interactionVFX;
    [SerializeField] SpriteRenderer _interactionVFXRenderer;
    MaterialPropertyBlock _interactionVFXMaterialPropertyBlock;


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

    [Header("Cat Transformation Settings")]
    [SerializeField] SpriteRenderer _characterSpriteRenderer;
    [SerializeField] CapsuleCollider2D _characterCollider;
    [SerializeField] SpriteRenderer _catSpriteRenderer;
    [SerializeField] Collider2D _catCollider;
    [SerializeField] AbilitySO _catAbilitySO;
    bool _isInCat;

    public bool IsInCat => _isInCat;

    void Awake()
    {
        _interactionVFXMaterialPropertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        _rigidbody2D.gravityScale = 0f; // Disable default gravity
        _normalGravity = (2f * _jumpHeight) / (_timeToApex * _timeToApex);
        _jumpVelocity = (2f * _jumpHeight) / _timeToApex;

    }




    void Update()
    {
        HandleHoverInteractable();
        HandleInteraction();

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

        bool lastGripping = _isGripping;
        _isGripping = _canGrip && !_isGrounded && _rigidbody2D.linearVelocity.y <= 0 && ((_isWallRight && _moveInput.x > 0) || (_isWallLeft && _moveInput.x < 0));
        if (_isGripping != lastGripping && _isGripping)
            _animatorController.GrabWall(_isWallRight ? -1 : 1);

        // Base Behavior Checks
        bool lastGroundedState = _isGrounded;
        _isGrounded = CheckGrounded();

        _animatorController.Grounded(_isGrounded);

        _isWallRight = CheckWall(Vector2.right);
        _isWallLeft = CheckWall(Vector2.left);

        if (_isJumping && _isGrounded)
        {
            // _isGrounded = false;

            if (_rigidbody2D.linearVelocity.y <= 0)
            {
                _isJumping = false;
                _animatorController.IsJumping(_isJumping);
            }
        }

        if (_isJumping && _isGrounded && !lastGroundedState)
        {
            _isJumping = false;
            _animatorController.IsJumping(_isJumping);
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

        if (_isEnchanting)
            return;


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

        _animatorController.Move(velocity.x);
    }
    void HandleGravity()
    {

        // Claws Grip Logic
        if (_isGripping)
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, 0f);
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
        _animatorController.IsJumping(_isJumping);
        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpVelocity * _jumpBoostMultiplier);
        _animatorController.Jumped();


    }

    void WallJump()
    {
        float direction = _isWallRight ? -1f : 1f;
        _isJumping = true;
        _animatorController.IsJumping(_isJumping);
        _isGripping = false;
        _wallJumpLockTimer = _wallJumpLockTime;
        _rigidbody2D.linearVelocity = new Vector2(direction * _wallJumpVelocity.x, _wallJumpVelocity.y);
        _animatorController.Jumped();
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
        // Cat is smaller so smaller rays
        Vector2 offset = _isInCat ? _catWallCheckOffset : _wallCheckOffset;
        int raysPerSide = _isInCat ? _catRaysPerSide : _raysPerSide;

        float size = (raysPerSide - 1) * _wallCheckSpacing;
        Vector2 bottom = (Vector2)transform.position + offset - Vector2.up * (size / 2);

        for (int i = 0; i < raysPerSide; i++)
        {
            Vector2 origin = bottom + Vector2.up * (i * _wallCheckSpacing);
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
            _vfxEnchant.PlayEnchantmentVFX(ability);
        }
    }


    public void ManageAbility(bool value)
    {
        _isEnchanting = value;
        _currentAbilityKeys.Clear();
        _vfxEnchant.VFXKeyList(_currentAbilityKeys);
    }

    public void AddAbilityKey(AbilityKey key)
    {
        if (!_isEnchanting) return;

        if (_isInCat)
        {
            // Search next index
            int nextIndex = _currentAbilityKeys.Count;
            List<AbilityKey> catAbilityKeys = _catAbilitySO.AbilityKeys;
            if (key != catAbilityKeys[nextIndex])
                return;
            _currentAbilityKeys.Add(key);
            _vfxEnchant.VFXUseKey(key);
            if (_currentAbilityKeys.Count >= catAbilityKeys.Count)
            {
                // Stay a cat if the character doesn't fit
                if (CanTransformBack())
                    TransformToCat(false);
                _currentAbilityKeys.Clear();
            }
            _vfxEnchant.VFXKeyList(_currentAbilityKeys);
        }
        else
        {
            _currentAbilityKeys.Add(key);
            _vfxEnchant.VFXUseKey(key);
            if (_currentAbilityKeys.Count >= 3)
            {
                HandleAbilityInput(_currentAbilityKeys);
                _currentAbilityKeys.Clear();
            }
            _vfxEnchant.VFXKeyList(_currentAbilityKeys);
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

    public void TransformToCat(bool isCat)
    {
        _isInCat = isCat;
        _characterSpriteRenderer.enabled = !isCat;
        _catSpriteRenderer.enabled = isCat;
        _characterCollider.enabled = !isCat;
        _catCollider.enabled = isCat;
    }

    bool CanTransformBack()
    {
        Vector2 point = _characterCollider.transform.TransformPoint(_characterCollider.offset);
        Vector2 size = _characterCollider.size * 0.9f;
        return Physics2D.OverlapCapsule(point, size, _characterCollider.direction, 0f, _groundLayer | _obstacleLayer) == null;
    }


    #endregion Ability System


    #region Interaction

    List<Interactable> SearchInteractables()
    {
        Vector2 worldCenter = (Vector2)transform.position + _interactionOffset;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldCenter, _interactionRange, _interactableLayer);
        List<Interactable> interactables = new List<Interactable>();
        foreach (var collider in colliders)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null)
                interactables.Add(interactable);
        }
        return interactables;
    }

    Interactable GetClosestInteractable()
    {
        List<Interactable> interactables = SearchInteractables();
        Interactable closest = null;
        float closestDistance = float.MaxValue;
        Vector2 worldCenter = (Vector2)transform.position + _interactionOffset;
        foreach (var interactable in interactables)
        {
            float distance = Vector2.Distance(worldCenter, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }
        return closest;
    }

    void HandleHoverInteractable()
    {
        Interactable hoverInteractable = GetClosestInteractable();

        if (hoverInteractable == _hoverInteractable)
            return;

        if (_hoverInteractable != null)
        {
            _hoverInteractable.EndHover();
            if (_currentInteractable == _hoverInteractable)
                _currentInteractable = null;
        }

        _hoverInteractable = hoverInteractable;

        if (_hoverInteractable != null)
        {
            _hoverInteractable.StartHover();

            _interactionVFX.SetActive(true);

            if (_hoverInteractable.InteractDuration > 0f)
                UpdateInteractionVFX(0f);
            else
                UpdateInteractionVFX(1f);
        }
        else
        {
            _interactionVFX.SetActive(false);
            UpdateInteractionVFX(1f);
        }
    }

    public void Interact(bool interact)
    {
        _interact = interact;
        if (interact)
        {
            // Press for interaction
            if (_hoverInteractable != null)
            {
                _currentInteractable = _hoverInteractable;
                _currentInteractable.StartInteract(this);
                _interactionTime = 0f;
            }
        }
        else
        {
            // Release interaction
            if (_currentInteractable != null)
            {
                _currentInteractable.StopInteract();
                _currentInteractable = null;
                _hoverInteractable = null;
                _interactionTime = 0f;
            }
        }
    }

    void HandleInteraction()
    {
        if (!_interact || _currentInteractable == null)
            return;
        UpdateInteractionVFX(_currentInteractable.CurrentInteractDuration / _currentInteractable.InteractDuration);
        _currentInteractable.HandleInteraction(this);
    }

    void UpdateInteractionVFX(float fill)
    {
        fill = Mathf.Clamp01(fill);
        _interactionVFXRenderer.SetPropertyBlock(_interactionVFXMaterialPropertyBlock);
        _interactionVFXMaterialPropertyBlock.SetFloat("_Fill", fill);
        _interactionVFXRenderer.SetPropertyBlock(_interactionVFXMaterialPropertyBlock);
    }

    #endregion Interaction


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

        // Gizmos.color = Color.blue;
        // for (int i = -1; i <= 1; i++)
        // {
        //     Vector2 origin = (Vector2)transform.position + _wallCheckOffset + Vector2.up * (i * _wallCheckSpacing);
        //     Gizmos.DrawLine(origin, origin + Vector2.right * _wallCheckDistance);
        //     Gizmos.DrawLine(origin, origin + Vector2.left * _wallCheckDistance);
        // }

        Vector2 hoverPosition = (Vector2)transform.position + _interactionOffset;
        Gizmos.DrawWireSphere(hoverPosition, _interactionRange);

        Gizmos.color = Color.purple;
        float size = (_raysPerSide - 1) * _wallCheckSpacing;
        Vector2 bottom = (Vector2)transform.position + _wallCheckOffset - Vector2.up * (size / 2);

        for (int i = 0; i < _raysPerSide; i++)
        {
            Vector2 origin = bottom + Vector2.up * (i * _wallCheckSpacing);
            Gizmos.DrawLine(origin, origin + Vector2.right * _wallCheckDistance);
            Gizmos.DrawLine(origin, origin + Vector2.left * _wallCheckDistance);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput; 

    [SerializeField] private PlayerCharacter _playerCharacter;

    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _enchantAction;

    InputAction _key1Action;
    InputAction _key2Action;
    InputAction _key3Action;

    InputAction _interactAction;

    InputAction _escapeAction;

    private void Awake()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _enchantAction = _playerInput.actions["Enchant"];
        _key1Action = _playerInput.actions["Key1"];
        _key2Action = _playerInput.actions["Key2"];
        _key3Action = _playerInput.actions["Key3"];
        _interactAction = _playerInput.actions["Interact"];
        _escapeAction = _playerInput.actions["Escape"];
    }

    void Update()
    {
        if (_escapeAction.WasPressedThisFrame())
        {
            SettingsManager.Instance.ShowSettings(!SettingsManager.Instance.IsSettingsVisible);
        }

        if(SettingsManager.Instance.IsSettingsVisible)
            return;

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _playerCharacter.MoveInput(moveInput);

        if (_jumpAction.WasPressedThisFrame())
        {
            // Handle jump logic here
            _playerCharacter.TryJump();

        }

        if (_enchantAction.WasPressedThisFrame())
        {
            _playerCharacter.ManageAbility(true);
        }
        else if (_enchantAction.WasReleasedThisFrame())
        {
            _playerCharacter.ManageAbility(false);
        }

        if (_key1Action.WasPressedThisFrame())
        {
            _playerCharacter.AddAbilityKey(AbilityKey.Key1);
        }

        if (_key2Action.WasPressedThisFrame())
        {
            _playerCharacter.AddAbilityKey(AbilityKey.Key2);
        }

        if (_key3Action.WasPressedThisFrame())
        {
            _playerCharacter.AddAbilityKey(AbilityKey.Key3);
        }
        
        if (_interactAction.WasPressedThisFrame())
        {
            _playerCharacter.Interact(true);
        }
        else if (_interactAction.WasReleasedThisFrame())
        {
            _playerCharacter.Interact(false);
        }
    }


}
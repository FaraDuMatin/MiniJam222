using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] Collider2D _collider;
    [SerializeField] float _interactDuration = 0f;
    float _currentInteractDuraction = 0f;
    [SerializeField] bool _multipleInteraction = false;
    bool _hasCompletedInteraction = false;
    bool _isInteracted = false;
    protected PlayerCharacter m_interactingCharacter; 

    [Header("Visual Settings")]
    [SerializeField] SpriteRenderer _visual;
    Sprite _defaultSprite;
    [SerializeField] Sprite _hoverSprite;

    [Header("Sound Effects")]
    [SerializeField] SoundSO _interactionSound;

    public float InteractDuration => _interactDuration;
    public float CurrentInteractDuration => _currentInteractDuraction;

    protected virtual void Start()
    {
        _defaultSprite = _visual.sprite;
        _collider.isTrigger = true;
    }

    void Update()
    {
        if(!_isInteracted)
            return;

        _currentInteractDuraction += Time.deltaTime;
        if(_currentInteractDuraction >= _interactDuration)
        {
            if(!_multipleInteraction && _hasCompletedInteraction)
                return;
            _hasCompletedInteraction = true;

            SoundObject sound = SoundManager.Instance.CreateSound(_interactionSound);
            sound.transform.position = this.transform.position;
            CompleteInteract();
        }
    }

    public void StartInteract(PlayerCharacter character)
    {
        _isInteracted = true;
        _currentInteractDuraction = 0f;
        m_interactingCharacter = character;
    }

    public virtual void HandleInteraction(PlayerCharacter character)
    {
        
    }

    public void StopInteract()
    {
        _isInteracted = false;
        m_interactingCharacter = null;
        _currentInteractDuraction = 0f;
    }

    protected virtual void CompleteInteract()
    {
        
    }

    public virtual void StartHover()
    {
        _visual.sprite = _hoverSprite;
    }

    public virtual void EndHover()
    {
        _visual.sprite = _defaultSprite;
    }

}

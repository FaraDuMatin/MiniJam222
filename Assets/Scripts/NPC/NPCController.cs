using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 7f;
    [SerializeField] NPCPathPoint _startPathPoint;
    [SerializeField] NPCPathPoint _endPatrolPathPoint;
    [SerializeField] float _patrolReachDistance = 0.5f;
    [SerializeField] float _idleTime = 2f;

    [Header("View")]
    [SerializeField] NPCView _npcView;
    [SerializeField] float _reachedPlayerDistance = 0.25f;
    [SerializeField] float _detectionTime = 1.5f;
    [SerializeField] float _catDetectionMultiplier = 0.5f;

    [Header("VFX")]
    [SerializeField] SpriteRenderer _characterRenderer;
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _intriguedRenderer;
    [SerializeField] SpriteRenderer _intriguedBackgroundRenderer;
    [SerializeField] Sprite _questionMarkSprite;
    [SerializeField] Color _questionColor;
    [SerializeField] Color _questionBackgroundColor;


    [Header("Sound")]
    [SerializeField] SoundSO _questionSound;
    [SerializeField] SoundSO _alertSound;


    [SerializeField] Sprite _exclamationMarkSprite;
    [SerializeField] Color _exclamationColor;
    [SerializeField] Color _exclamationBackgroundColor;
    MaterialPropertyBlock _vfxIntriguedMpb;

    public NPCPathPoint StartPathPoint => _startPathPoint;
    public NPCPathPoint EndPatrolPathPoint => _endPatrolPathPoint;
    public float IdleTime => _idleTime;

    public SpriteRenderer CharacterRenderer => _characterRenderer;
    public Animator Animator => _animator;
    public NPCView NPCView => _npcView;
    public float ReachedPlayerDistance => _reachedPlayerDistance;
    public float DetectionTime => _detectionTime;
    public float CatDetectionMultiplier => _catDetectionMultiplier;
    public bool PlayerIsCat => _npcView.CharacterTarget != null && _npcView.CharacterTarget.IsInCat;

    NPCStateMachine _stateMachine;

    public bool HasPatrolBehaviour => _startPathPoint != null && _endPatrolPathPoint != null;

    void Awake()
    {
        if (_startPathPoint != null)
            this.transform.position = _startPathPoint.GetPosition();
        _stateMachine = new NPCStateMachine(this);

        _vfxIntriguedMpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }

    public bool ReachedPoint(INPCPathPoint point)
    {
        return ReachedPosition(point.GetPosition());
    }

    public bool ReachedPosition(Vector3 position)
    {
        return Vector3.Distance(this.transform.position, position) <= _patrolReachDistance;
    }

    public void Move(Vector2 direction, float elapsedTime)
    {
        Vector3 move = new Vector3(direction.x, direction.y, 0) * _moveSpeed * elapsedTime;
        this.transform.Translate(move, Space.World);
        Face(direction);
    }

    public void Face(Vector2 direction)
    {
        if (direction.x == 0)
            return;
        _characterRenderer.flipX = direction.x < 0;
        _npcView.Flip(direction.x < 0);
    }

    public bool SeePlayer()
    {
        return _npcView.CharacterTarget != null && _npcView.CanSeeIt;
    }

    public void UpdateIntriguedFill(float fillAmount)
    {
        _intriguedRenderer.sprite = _questionMarkSprite;
        _vfxIntriguedMpb.SetFloat("_Fill", fillAmount);
        _intriguedRenderer.SetPropertyBlock(_vfxIntriguedMpb);
    }

    public void HideMark()
    {
        _intriguedRenderer.sprite = null;
        _intriguedBackgroundRenderer.sprite = null;
    }

    public void QuestionMark()
    {
        _intriguedRenderer.sprite = _questionMarkSprite;
        _intriguedRenderer.color = _questionColor;

        _intriguedBackgroundRenderer.sprite = _questionMarkSprite;
        _intriguedBackgroundRenderer.color = _questionBackgroundColor;
        PlaySound(_questionSound);

    }

    public void ExclamationMark()
    {
        _intriguedRenderer.sprite = _exclamationMarkSprite;
        _intriguedRenderer.color = _exclamationColor;

        _intriguedBackgroundRenderer.sprite = _exclamationMarkSprite;
        _intriguedBackgroundRenderer.color = _exclamationBackgroundColor;
        PlaySound(_alertSound);

    }

    public Vector3 PlayerPosition => _npcView.CharacterTarget.transform.position;

    void PlaySound(SoundSO sound)
    {
        if (sound == null)
            return;
        sound.Play().transform.position = transform.position;
    }

    public bool ReachedPlayer()
    {
        return Vector3.Distance(this.transform.position, PlayerPosition) <= _reachedPlayerDistance;
    }
}
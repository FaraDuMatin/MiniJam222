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

    [Header("VFX")]
    [SerializeField] SpriteRenderer _characterRenderer;
    [SerializeField] Animator _animator;

    public NPCPathPoint StartPathPoint => _startPathPoint;
    public NPCPathPoint EndPatrolPathPoint => _endPatrolPathPoint;
    public float IdleTime => _idleTime;

    public SpriteRenderer CharacterRenderer => _characterRenderer;
    public Animator Animator => _animator;
    public NPCView NPCView => _npcView;
    public float ReachedPlayerDistance => _reachedPlayerDistance;
    
    NPCStateMachine _stateMachine;

    public bool HasPatrolBehaviour => _startPathPoint != null && _endPatrolPathPoint != null;

    void Awake()
    {
        if(_startPathPoint != null)
            this.transform.position = _startPathPoint.GetPosition();
        _stateMachine = new NPCStateMachine(this);
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

        if(direction.x != 0)
        {
            _characterRenderer.flipX = direction.x < 0;
            _npcView.Flip(direction.x < 0);
        }
    }

    public bool SeePlayer()
    {
        return _npcView.CharacterTarget != null && _npcView.CanSeeIt;
    }

    public Vector3 PlayerPosition => _npcView.CharacterTarget.transform.position;
}
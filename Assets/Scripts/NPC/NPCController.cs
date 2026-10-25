using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 7f;
    [SerializeField] NPCPathPoint _startPathPoint;
    [SerializeField] NPCPathPoint _endPatrolPathPoint;
    [SerializeField] float _patrolReachDistance = 0.5f;
    [SerializeField] float _idleTime = 2f;

    [Header("VFX")]
    [SerializeField] SpriteRenderer _characterRenderer;
    [SerializeField] Animator _animator;

    public NPCPathPoint StartPathPoint => _startPathPoint;
    public NPCPathPoint EndPatrolPathPoint => _endPatrolPathPoint;
    public float IdleTime => _idleTime;

    public SpriteRenderer CharacterRenderer => _characterRenderer;
    public Animator Animator => _animator;

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
        return Vector3.Distance(this.transform.position, point.GetPosition()) <= _patrolReachDistance;
    }

    public void Move(Vector2 direction, float elapsedTime)
    {
        Vector3 move = new Vector3(direction.x, direction.y, 0) * _moveSpeed * elapsedTime;
        this.transform.Translate(move, Space.World);
    }
}
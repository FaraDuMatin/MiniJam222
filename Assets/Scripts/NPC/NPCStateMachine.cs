using UnityEngine;
using System.Collections.Generic;

public class NPCStateMachine
{
    NPCController _controller;

    NPCState _currentState;

    NPCIdleState _idleState;
    NPCPatrolState _patrolState;
    NPCFollowPlayer _followPlayerState;
    NPCIntriguedState _intriguedState;

    public NPCIdleState IdleState => _idleState;
    public NPCPatrolState PatrolState => _patrolState;
    public NPCFollowPlayer FollowPlayerState => _followPlayerState;
    public NPCIntriguedState IntriguedState => _intriguedState;

    public NPCStateMachine(NPCController controller)
    {
        _controller = controller;
        _idleState = new NPCIdleState(_controller, this);
        _patrolState = new NPCPatrolState(_controller, this);
        _followPlayerState = new NPCFollowPlayer(_controller, this);
        _intriguedState = new NPCIntriguedState(_controller, this);

        ChangeState(_idleState);
    }

    public void ChangeState(NPCState newState)
    {
        if(_currentState != null)
            _currentState.Exit();

        _currentState = newState;

        if(newState != null)
            _currentState.Enter();
    }

    public void Update(float deltaTime)
    {
        if(_currentState != _followPlayerState && _currentState != _intriguedState && _controller.SeePlayer())
            ChangeState(_intriguedState);

        if(_currentState != null)
            _currentState.Update(deltaTime);
    }
}

public abstract class NPCState
{
    protected NPCController m_controller;
    protected NPCStateMachine m_stateMachine;

    public NPCState(NPCController controller, NPCStateMachine stateMachine)
    {
        m_controller = controller;
        m_stateMachine = stateMachine;
    }
    public abstract void Enter();
    public abstract void Update(float deltaTime);
    public abstract void Exit();
}

public class NPCIdleState : NPCState
{
    float _idleTimer;
    public NPCIdleState(NPCController controller, NPCStateMachine stateMachine) : base(controller, stateMachine)
    {
    }
    public override void Enter()
    {
        _idleTimer = 0f;
        m_controller.Animator.SetBool("Running", false);
        m_controller.HideMark();
    }
    public override void Update(float deltaTime)
    {
        _idleTimer += deltaTime;
        if(_idleTimer >= m_controller.IdleTime)
        {
            if(m_controller.HasPatrolBehaviour)
                m_stateMachine.ChangeState(m_stateMachine.PatrolState);
            else
                _idleTimer = 0f;
        }
    }
    public override void Exit()
    {
        _idleTimer = 0f;
    }
}

public class NPCPatrolState : NPCState
{
    INPCPathPoint _currentPatrolTarget;
    List<INPCPathPoint> _patrolPathPoints = new List<INPCPathPoint>();
    int _currentPatrolIndex = 0;

    public NPCPatrolState(NPCController controller, NPCStateMachine stateMachine) : base(controller, stateMachine)
    {
        _currentPatrolTarget = m_controller.EndPatrolPathPoint;
    }

    public override void Enter()
    {
        m_controller.HideMark();

        if(!m_controller.HasPatrolBehaviour || m_controller.StartPathPoint == null || m_controller.EndPatrolPathPoint == null)
        {
            m_stateMachine.ChangeState(m_stateMachine.IdleState);
            return;
        }

        // Get the next patrol path point from the controller
        _patrolPathPoints.Clear();
        _currentPatrolIndex = 0;
        
        if(_currentPatrolTarget == null)
            _currentPatrolTarget = m_controller.EndPatrolPathPoint;

        List<INPCPathPoint> path = NPCPathManager.Instance.GetPathTo(m_controller.transform.position, _currentPatrolTarget);
        if(path != null)
            _patrolPathPoints.AddRange(path);
        else
        {
            m_stateMachine.ChangeState(m_stateMachine.IdleState);
            return;
        }

        m_controller.Animator.SetBool("Running", true);
    }
    public override void Update(float deltaTime)
    {
        INPCPathPoint currentTarget = _patrolPathPoints[_currentPatrolIndex];
        Vector2 direction = (currentTarget.GetPosition() - m_controller.transform.position).normalized;
        m_controller.Move(direction, deltaTime);

        bool reachedPoint = m_controller.ReachedPoint(currentTarget);
        if(reachedPoint)
        {
            _currentPatrolIndex++;
            currentTarget.EnterPosition(m_controller.transform);
            if(_currentPatrolIndex >= _patrolPathPoints.Count)
            {
                _currentPatrolTarget = LoopPatrol(_currentPatrolTarget);
                m_stateMachine.ChangeState(m_stateMachine.IdleState);
            }
        }
    }

    public override void Exit()
    {
        _patrolPathPoints.Clear();
        _currentPatrolIndex = 0;
        m_controller.Animator.SetBool("Running", false);
    }

    INPCPathPoint LoopPatrol(INPCPathPoint currentTarget)
    {
        if(currentTarget.GetGameObject() == m_controller.EndPatrolPathPoint.GetGameObject())
            return m_controller.StartPathPoint;
        else if(currentTarget.GetGameObject() == m_controller.StartPathPoint.GetGameObject())
            return m_controller.EndPatrolPathPoint;
        return currentTarget;
    }
}

public class NPCFollowPlayer : NPCState
{
    const float ReplanInterval = 0.5f;

    List<INPCPathPoint> _pathToPlayer = new List<INPCPathPoint>();
    int _currentIndex = 0;
    Vector3 _approachPoint;
    float _replanTimer = 0f;

    public NPCFollowPlayer(NPCController controller, NPCStateMachine stateMachine) : base(controller, stateMachine) {}

    public override void Enter()
    {
        m_controller.Animator.SetBool("Running", true);
        Replan();

        m_controller.UpdateIntriguedFill(1f);
        m_controller.ExclamationMark();
    }

    public override void Update(float deltaTime)
    {
        if(!m_controller.SeePlayer())
        {
            m_stateMachine.ChangeState(m_stateMachine.IdleState);
            return;
        }

        if(m_controller.ReachedPlayer())
        {
            Vector2 directionToPlayer = ((Vector3)m_controller.PlayerPosition - m_controller.transform.position).normalized;
            m_controller.Face(directionToPlayer);
            return;
        }

        _replanTimer -= deltaTime;
        if(_replanTimer <= 0f)
            Replan();

        Vector3 currentTarget = CurrentTargetPosition();
        Vector2 direction = ((Vector3)currentTarget - m_controller.transform.position).normalized;
        m_controller.Move(direction, deltaTime);

        if(_currentIndex < _pathToPlayer.Count && m_controller.ReachedPoint(_pathToPlayer[_currentIndex]))
        {
            _pathToPlayer[_currentIndex].EnterPosition(m_controller.transform);
            _currentIndex++;
        }
    }

    public override void Exit()
    {
        m_controller.Animator.SetBool("Running", false);
        _pathToPlayer.Clear();
        _currentIndex = 0;
        m_controller.HideMark();
    }

    Vector3 CurrentTargetPosition()
    {
        // On the final leg, steer straight at the real approach point instead of
        // waiting to fully arrive at the last node first - otherwise the NPC can
        // face away from the player while walking to that node and lose sight of them.
        bool onFinalLeg = _currentIndex >= _pathToPlayer.Count - 1;
        if(!onFinalLeg)
            return _pathToPlayer[_currentIndex].GetPosition();
        return _approachPoint;
    }

    void Replan()
    {
        _replanTimer = ReplanInterval;
        _pathToPlayer = NPCPathManager.Instance.GetPathToClosestApproach(
            m_controller.transform.position,
            m_controller.PlayerPosition,
            out _approachPoint) ?? new List<INPCPathPoint>();
        _currentIndex = 0;
    }
}

public class NPCIntriguedState : NPCState
{
    float _timer;

    public NPCIntriguedState(NPCController controller, NPCStateMachine stateMachine) : base(controller, stateMachine) {}

    public override void Enter()
    {
        _timer = 0f;
        m_controller.Animator.SetBool("Running", false);

        m_controller.UpdateIntriguedFill(0f);
        m_controller.QuestionMark();
    }

    public override void Update(float deltaTime)
    {
        if(!m_controller.SeePlayer())
        {
            m_stateMachine.ChangeState(m_stateMachine.IdleState);
            return;
        }

        Vector2 direction = ((Vector3)m_controller.PlayerPosition - m_controller.transform.position).normalized;
        m_controller.Face(direction);

        float rate = m_controller.PlayerIsCat ? m_controller.CatDetectionMultiplier : 1f;
        _timer += deltaTime * rate;

        m_controller.UpdateIntriguedFill(_timer / m_controller.DetectionTime);

        if(_timer >= m_controller.DetectionTime)
            m_stateMachine.ChangeState(m_stateMachine.FollowPlayerState);
    }

    public override void Exit()
    {
        _timer = 0f;
        m_controller.HideMark();
    }
}
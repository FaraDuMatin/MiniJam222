using UnityEngine;
using System.Collections.Generic;

public class NPCStateMachine
{
    NPCController _controller;

    NPCState _currentState;

    NPCIdleState _idleState;
    NPCPatrolState _patrolState;

    public NPCIdleState IdleState => _idleState;
    public NPCPatrolState PatrolState => _patrolState;

    public NPCStateMachine(NPCController controller)
    {
        _controller = controller;
        _idleState = new NPCIdleState(_controller, this);
        _patrolState = new NPCPatrolState(_controller, this);

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

        float vfxDir = direction.x;
        if(vfxDir == 0)
            return;
        m_controller.CharacterRenderer.flipX = vfxDir < 0;
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
    public NPCFollowPlayer(NPCController controller, NPCStateMachine stateMachine) : base(controller, stateMachine) {}

    public override void Enter()
    {
        m_controller.Animator.SetBool("Running", true);
    }

    public override void Update(float deltaTime)
    {
        if(m_controller.SeePlayer())
        {
            // Move closer to the player by using the NPCPathPoints (NPCController has access to points from it's starting point)
        }
        else
        {
            m_stateMachine.ChangeState(m_stateMachine.IdleState);
        }
    }

    public override void Exit()
    {
        m_controller.Animator.SetBool("Running", false);
    }
}
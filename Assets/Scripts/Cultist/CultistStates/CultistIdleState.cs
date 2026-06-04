using UnityEngine;

public class CultistIdleState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    public CultistIdleState(CultistStateMachine _stateMachine, CultistController _enemyController)
    {
        cultistController = _enemyController;
        cultistStateMachine = _stateMachine;
    }

    public void Enter()
    {
        cultistController.Stop();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}

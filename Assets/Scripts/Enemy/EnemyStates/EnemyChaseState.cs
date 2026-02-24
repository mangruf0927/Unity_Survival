using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private readonly EnemyController enemyController;
    private readonly EnemyStateMachine stateMachine;

    public EnemyChaseState(EnemyStateMachine _stateMachine, EnemyController _enemyController)
    {
        enemyController = _enemyController;
        stateMachine = _stateMachine;
    }

    public void Enter()
    {
        
    }   

    public void Update()
    {
        if(enemyController.RangeCheck()) enemyController.Chase();
        else stateMachine.ChangeState(EnemyStateEnums.IDLE);
    }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        
    }
}

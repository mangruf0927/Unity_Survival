using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private readonly EnemyController enemyController;
    private readonly EnemyStateMachine stateMachine;

    public EnemyPatrolState(EnemyStateMachine _stateMachine, EnemyController _enemyController)
    {
        enemyController = _enemyController;
        stateMachine = _stateMachine;
    }

    public void Enter()
    {
        enemyController.Patrol();
    }   

    public void Update()
    {
        if(enemyController.CanChasePlayer()) stateMachine.ChangeState(EnemyStateEnums.CHASE);
        else if(enemyController.CheckArrive()) stateMachine.ChangeState(EnemyStateEnums.IDLE);
    }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        
    }
}

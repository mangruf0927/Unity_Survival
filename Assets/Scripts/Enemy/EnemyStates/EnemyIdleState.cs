using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private EnemyController enemyController;
    private EnemyStateMachine stateMachine;

    public EnemyIdleState(EnemyStateMachine _stateMachine, EnemyController _enemyController)
    {
        enemyController = _enemyController;
        stateMachine = _stateMachine;
    }

    public void Enter()
    {
        enemyController.Stop();
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

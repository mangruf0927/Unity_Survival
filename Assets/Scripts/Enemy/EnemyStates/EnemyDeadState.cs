using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    private readonly EnemyController enemyController;
    private readonly EnemyStateMachine stateMachine;

    public EnemyDeadState(EnemyStateMachine _stateMachine, EnemyController _enemyController)
    {
        enemyController = _enemyController;
        stateMachine = _stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Dead State");
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

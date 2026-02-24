using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private readonly EnemyController enemyController;
    private readonly EnemyStateMachine stateMachine;

    private float timer;
    private float randomTime;

    public EnemyIdleState(EnemyStateMachine _stateMachine, EnemyController _enemyController)
    {
        enemyController = _enemyController;
        stateMachine = _stateMachine;
    }

    public void Enter()
    {
        enemyController.Stop();

        timer = 0f;
        randomTime = enemyController.RandomTime(); 
    }

    public void Update()
    {
        if (enemyController.CanChasePlayer())
        {
            stateMachine.ChangeState(EnemyStateEnums.CHASE);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= randomTime) stateMachine.ChangeState(EnemyStateEnums.PATROL);
    }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        
    }
}

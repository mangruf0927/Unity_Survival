using UnityEngine;

public class EnemyStateMachine : StateMachine<EnemyStateEnums, IEnemyState>
{
    [SerializeField] private EnemyController enemyController;
    protected override EnemyStateEnums InitialState => EnemyStateEnums.IDLE;

    protected override void InitializeStates()
    {
        AddState(EnemyStateEnums.IDLE, new EnemyIdleState(this, enemyController));
        AddState(EnemyStateEnums.CHASE, new EnemyChaseState(this, enemyController));
        AddState(EnemyStateEnums.PATROL, new EnemyPatrolState(this, enemyController));
        AddState(EnemyStateEnums.DEAD, new EnemyDeadState(this, enemyController));
    }
}

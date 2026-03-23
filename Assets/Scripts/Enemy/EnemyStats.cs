using UnityEngine;

public class EnemyStats : HealthBase
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyStateMachine enemyStateMachine;

    public override int MaxHP => enemyData.maxHP;
    public int AttackDamage => enemyData.attackDamage;
    public float ScanRange => enemyData.scanRange;
    public bool CanChase => enemyData.canChase;
    public float PatrolRange => enemyData.patrolRange;

    protected override void Die()
    {
        enemyStateMachine.ChangeState(EnemyStateEnums.DEAD);
    }
}

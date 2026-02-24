using UnityEngine;

public class EnemyStats : HealthBase
{
    [SerializeField] private EnemyData data;
    [SerializeField] private EnemyStateMachine enemyStateMachine;

    protected override int MaxHP => data.maxHP;
    public int AttackDamage => data.attackDamage;
    public float ScanRange => data.scanRange;
    public bool CanChase => data.canChase;
    public float PatrolRange => data.patrolRange;

    protected override void Die()
    {
        enemyStateMachine.ChangeState(EnemyStateEnums.DEAD);
    }
}

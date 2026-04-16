using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyStateMachine enemyStateMachine;
    [SerializeField] private Transform hpBarPoint;
    [SerializeField] private EnemyHPBarController hpBarController;

    public int MaxHP => enemyData.maxHP;
    public int AttackDamage => enemyData.attackDamage;
    public float ScanRange => enemyData.scanRange;
    public bool CanChase => enemyData.canChase;
    public float PatrolRange => enemyData.patrolRange;
    public int CurrentHP { get; private set; }
    public PoolTypeEnums EnemyType => enemyData.enemyType;
    public Transform HPBarPoint => hpBarPoint;

    public event Action<EnemyStats> OnDamaged;
    public event Action<EnemyStats> OnDead;

    private void OnEnable()
    {
        CurrentHP = MaxHP;
        hpBarController.Register(this);
        enemyStateMachine.ChangeState(EnemyStateEnums.IDLE);
    }

    private void OnDisable()
    {
        hpBarController.UnRegister(this);
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || CurrentHP <= 0) return;

        CurrentHP = Mathf.Max(CurrentHP - dmg, 0);
        OnDamaged?.Invoke(this);

        if (CurrentHP <= 0)
        {
            OnDead?.Invoke(this);
            Die();
        }
    }

    private void Die()
    {
        enemyStateMachine.ChangeState(EnemyStateEnums.DEAD);
    }
}

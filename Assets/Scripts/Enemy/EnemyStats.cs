using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] int enemyId;
    [SerializeField] private EnemyStateMachine enemyStateMachine;
    [SerializeField] private Transform hpBarPoint;
    [SerializeField] private EnemyHPBarController hpBarController;

    private string enemyName;
    private int maxHp;
    private int attackDamage;
    private float scanRange;
    private bool canChase;
    private float patrolRange;
    private float alertDuration;
    private PoolTypeEnums enemyType;

    public int EnemyId => enemyId;
    public string EnemyName => enemyName;
    public int MaxHp => maxHp;
    public int AttackDamage => attackDamage;
    public float ScanRange => scanRange;
    public bool CanChase => canChase;
    public float PatrolRange => patrolRange;
    public float AlertDuration => alertDuration;
    public PoolTypeEnums EnemyType => enemyType;
    public Transform HPBarPoint => hpBarPoint;

    public int CurrentHp { get; private set; }

    public event Action<EnemyStats> OnDamaged;
    public event Action<EnemyStats> OnDead;

    private void OnDisable()
    {
        hpBarController.UnRegister(this);
        OnDamaged = null;
        OnDead = null;
    }

    public void SetUp(EnemyData data)
    {
        enemyName = data.Name;
        maxHp = data.MaxHp;
        attackDamage = data.AttackDamage;
        scanRange = data.ScanRange;
        canChase = data.CanChase;
        patrolRange = data.PatrolRange;
        alertDuration = data.AlertDuration;
        enemyType = data.EnemyType;

        CurrentHp = MaxHp;

        hpBarController.Register(this);
        enemyStateMachine.ChangeState(EnemyStateEnums.IDLE);
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || CurrentHp <= 0) return;

        CurrentHp = Mathf.Max(CurrentHp - dmg, 0);
        OnDamaged?.Invoke(this);

        if (CurrentHp <= 0)
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

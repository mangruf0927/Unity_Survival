using UnityEngine;

public class EnemyStats : EnemyStatsBase
{
    private string enemyName;
    private int attackDamage;
    private float scanRange;
    private bool canChase;
    private float patrolRange;
    private float alertDuration;
    private PoolTypeEnums enemyType;

    public int EnemyId => Id;
    public string EnemyName => enemyName;
    public int AttackDamage => attackDamage;
    public float ScanRange => scanRange;
    public bool CanChase => canChase;
    public float PatrolRange => patrolRange;
    public float AlertDuration => alertDuration;
    public PoolTypeEnums EnemyType => enemyType;

    public void SetUp(EnemyData data)
    {
        if (data == null)
            return;

        enemyName = data.Name;
        attackDamage = data.AttackDamage;
        scanRange = data.ScanRange;
        canChase = data.CanChase;
        patrolRange = data.PatrolRange;
        alertDuration = data.AlertDuration;
        enemyType = data.EnemyType;

        InitializeHp(data.MaxHp);
    }
}
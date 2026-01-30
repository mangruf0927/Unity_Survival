using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;
    [SerializeField] private EnemyStateMachine enemyStateMachine;

    public int MaxHP => data.maxHP;
    public int AttackDamage => data.attackDamage;
    public float ScanRange => data.scanRange;
    public bool CanChase => data.canChase;
    public float PatrolRange => data.patrolRange;
    
    public int CurrentHP { get; private set; }

    private void Awake()
    {
        CurrentHP = data.maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if(dmg < 0) return;

        CurrentHP = Mathf.Max(CurrentHP - dmg, 0);
        Debug.Log(CurrentHP);
        if(CurrentHP <= 0) enemyStateMachine.ChangeState(EnemyStateEnums.DEAD);
    }
}

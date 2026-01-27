using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP;
    [SerializeField] private int attackDamage;
    [SerializeField] private EnemyStateMachine enemyStateMachine;
    
    public int CurrentHP { get; private set; }

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if(dmg < 0) return;

        CurrentHP = Mathf.Max(CurrentHP - dmg, 0);
        if(CurrentHP <= 0) enemyStateMachine.ChangeState(EnemyStateEnums.DEAD);
    }
}

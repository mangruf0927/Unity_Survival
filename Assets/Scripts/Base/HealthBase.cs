using UnityEngine;

public abstract class HealthBase : MonoBehaviour, IDamageable
{
    protected abstract int MaxHP { get; }
    public int CurrentHP { get; private set; }

    protected virtual void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int dmg)
    {
        if(dmg <= 0) return;

        CurrentHP = Mathf.Max(CurrentHP - dmg, 0);
        if(CurrentHP <= 0) Die();
    }

    protected abstract void Die();
}

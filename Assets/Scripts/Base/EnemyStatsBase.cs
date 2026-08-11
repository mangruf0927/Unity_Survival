using System;
using UnityEngine;

public abstract class EnemyStatsBase : MonoBehaviour, IDamageable
{
    [SerializeField] private int id;
    [SerializeField] private Transform hpBarPoint;

    private EnemyHPBarController hpBarController;
    protected int maxHp;

    public int Id => id;
    public int MaxHp => maxHp;
    public int CurrentHp { get; private set; }
    public Transform HPBarPoint => hpBarPoint;

    public event Action<EnemyStatsBase> OnDamaged;
    public event Action<EnemyStatsBase> OnDead;

    protected virtual void OnDisable()
    {
        if (hpBarController != null)
            hpBarController.UnRegister(this);

        OnDamaged = null;
        OnDead = null;
    }

    public void SetHPBarController(EnemyHPBarController controller)
    {
        hpBarController = controller;
    }

    protected void InitializeHp(int maxHp)
    {
        this.maxHp = maxHp;
        CurrentHp = maxHp;

        if (hpBarController != null)
            hpBarController.Register(this);
    }

    public void LoadHp(int hp)
    {
        CurrentHp = Mathf.Clamp(hp, 0, maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || CurrentHp <= 0)
            return;

        CurrentHp = Mathf.Max(CurrentHp - damage, 0);

        if (CurrentHp <= 0)
        {
            OnDead?.Invoke(this);
            return;
        }

        OnDamaged?.Invoke(this);
    }
}
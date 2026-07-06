using System;
using UnityEngine;

public class CultistStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int cultistId;
    [SerializeField] private CultistStateMachine cultistStateMachine;
    [SerializeField] private Transform hpBarPoint;
    [SerializeField] private CultistHPBarController hpBarController;

    private int maxHp;
    private float scanRange;
    private float maxRaidCenterDistance;
    private float returnDistance;
    private float returnSearchRange;
    private float alertDuration;
    private PoolTypeEnums cultistType;

    public int CultistId => cultistId;
    public int MaxHp => maxHp;
    public float ScanRange => scanRange;
    public float MaxRaidCenterDistance => maxRaidCenterDistance;
    public float ReturnDistance => returnDistance;
    public float ReturnSearchRange => returnSearchRange;
    public float AlertDuration => alertDuration;
    public PoolTypeEnums CultistType => cultistType;
    public Transform HPBarPoint => hpBarPoint;

    public int CurrentHp { get; private set; }

    public event Action<CultistStats> OnDamaged;
    public event Action<CultistStats> OnDead;

    private void OnDisable()
    {
        if (hpBarController != null) hpBarController.UnRegister(this);
        OnDamaged = null;
        OnDead = null;
    }

    public void SetUp(CultistData data)
    {
        if (data == null) return;

        maxHp = data.MaxHp;
        scanRange = data.ScanRange;
        maxRaidCenterDistance = data.MaxRaidCenterDistance;
        returnDistance = data.ReturnDistance;
        returnSearchRange = data.ReturnSearchRange;
        alertDuration = data.AlertDuration;
        cultistType = data.CultistType;

        CurrentHp = maxHp;

        if (hpBarController == null)
        {
            hpBarController = FindFirstObjectByType<CultistHPBarController>();
        }

        if (hpBarController != null) hpBarController.Register(this);
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
        cultistStateMachine.ChangeState(CultistStateEnums.DEAD);
    }
}

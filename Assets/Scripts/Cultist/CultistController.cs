using System;
using UnityEngine;
using UnityEngine.AI;

public class CultistController : MonoBehaviour
{
    [Serializable]
    private class WeaponData
    {
        public CultistWeaponType type;
        public GameObject prefab;
    }

    [SerializeField] private CultistStats cultistStats;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;
    [SerializeField] private CultistStateMachine cultistStateMachine;
    [SerializeField] private Transform equipPosition;
    [SerializeField] private CultistWeaponType weaponType;
    [SerializeField] private WeaponData[] weaponDatas;

    [SerializeField] private Transform target;
    [SerializeField] private Transform raidCenter;

    private GameObject currentWeapon;
    private CultistWeapon weapon;
    private float lastAttackTime = float.NegativeInfinity;
    private float alertEndTime;

    public Animator Animator => animator;
    public CultistWeapon Weapon => weapon;
    public bool IsAlerted => Time.time < alertEndTime;
    public PoolTypeEnums CultistType => cultistStats.CultistType;

    private void Awake()
    {
        SetWeapon(weaponType);
    }

    private void OnEnable()
    {
        if (cultistStats == null) return;
        cultistStats.OnDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        if (cultistStats == null) return;
        cultistStats.OnDamaged -= OnDamaged;
        alertEndTime = 0f;
    }

    private void OnDamaged(CultistStats stats)
    {
        if (stats.CurrentHp <= 0) return;

        Alert();
        cultistStateMachine.ChangeState(CultistStateEnums.CHASE);
    }

    public void SetUp(Transform player, Transform raidCenter)
    {
        target = player;
        this.raidCenter = raidCenter;

        CultistData data = DataManager.Instance.CultistTable.Get(cultistStats.CultistId);
        cultistStats.SetUp(data);

        alertEndTime = 0f;
        lastAttackTime = float.NegativeInfinity;

        animator.ResetTrigger("Dead");
        animator.SetLayerWeight(1, 1f);
        cultistStateMachine.ChangeState(CultistStateEnums.IDLE);
    }

    public void Stop()
    {
        navMesh.isStopped = true;
        navMesh.velocity = Vector3.zero;
        navMesh.ResetPath();
        animator.SetFloat("speed", 0f);
    }

    public void Alert()
    {
        alertEndTime = Time.time + cultistStats.AlertDuration;
    }

    public bool ShouldChasePlayer()
    {
        if (target == null) return false;

        return IsAlerted || CheckRange();
    }

    public bool CheckRange()
    {
        if (target == null) return false;

        float distance = (target.position - transform.position).sqrMagnitude;
        return distance <= cultistStats.ScanRange * cultistStats.ScanRange;
    }

    public void Chase()
    {
        if (target == null) return;

        navMesh.isStopped = false;
        navMesh.SetDestination(target.position);
    }

    public void ReturnToRaidCenter()
    {
        if (raidCenter == null) return;

        navMesh.isStopped = false;

        Vector3 direction = (transform.position - raidCenter.position).normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        Vector3 returnPosition = raidCenter.position + direction * cultistStats.ReturnDistance;

        if (NavMesh.SamplePosition(returnPosition, out var hit, cultistStats.ReturnSearchRange, NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
    }

    public bool CheckArrive()
    {
        if (navMesh.pathPending || !navMesh.hasPath) return false;
        return navMesh.remainingDistance <= navMesh.stoppingDistance + 0.1f;
    }

    public bool IsAwayFromRaidCenter()
    {
        if (raidCenter == null) return false;

        float distance = (raidCenter.position - transform.position).sqrMagnitude;
        return distance > cultistStats.MaxRaidCenterDistance * cultistStats.MaxRaidCenterDistance;
    }

    public bool CheckAttackRange()
    {
        if (target == null || weapon == null) return false;

        float distance = (target.transform.position - transform.position).sqrMagnitude;
        return distance <= weapon.AttackRange * weapon.AttackRange;
    }

    public bool CanAttack()
    {
        if (weapon == null) return false;
        return Time.time >= lastAttackTime + weapon.AttackCoolTime;
    }

    public void Attack()
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;
        weapon.Attack(target);
    }

    public void EndAttack()
    {
        if (weapon == null) return;
        weapon.EndAttack();
    }

    public void SetWeapon(CultistWeaponType weaponType)
    {
        this.weaponType = weaponType;

        if (weaponDatas == null) return;

        foreach (WeaponData data in weaponDatas)
        {
            if (data.type != weaponType) continue;

            AttachWeapon(data.prefab);
            return;
        }
    }

    private void AttachWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null || equipPosition == null) return;

        if (currentWeapon != null) Destroy(currentWeapon);

        currentWeapon = Instantiate(weaponPrefab);
        weapon = currentWeapon.GetComponentInChildren<CultistWeapon>();

        currentWeapon.transform.SetParent(equipPosition, true);

        if (weapon != null && weapon.AttachPoint != null)
        {
            currentWeapon.transform.rotation = equipPosition.rotation * Quaternion.Inverse(weapon.AttachPoint.rotation) * currentWeapon.transform.rotation;
            currentWeapon.transform.position += equipPosition.position - weapon.AttachPoint.position;
            return;
        }

        currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}

using System;
using UnityEngine;
using UnityEngine.AI;

public class CultistController : MonoBehaviour, IDamageable
{
    [Serializable]
    private class WeaponData
    {
        public CultistWeaponType type;
        public GameObject prefab;
    }

    [SerializeField] private Transform target;
    [SerializeField] private CampFire campFire;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;
    [SerializeField] private CultistStateMachine cultistStateMachine;

    // >>
    [SerializeField] private float scanRange;          // 플레이어 감지 범위
    [SerializeField] private float maxCampDistance;    // 플레이어를 놓쳤을 때 복귀를 판단하는 캠프 기준 거리
    [SerializeField] private float returnDistance;     // 캠프파이어 중심에서 떨어져 멈출 거리
    [SerializeField] private float returnSearchRange;  // 복귀 위치 주변에서 NavMesh 지점을 찾는 범위
    [SerializeField] private float alertDuration;

    [SerializeField] private Transform equipPosition;
    [SerializeField] private PoolTypeEnums cultistType;
    [SerializeField] private CultistWeaponType weaponType;
    [SerializeField] private WeaponData[] weaponDatas;

    [SerializeField] private int maxHP = 125;
    // <<

    private GameObject currentWeapon;
    private CultistWeapon weapon;
    private int currentHp;
    private float lastAttackTime = float.NegativeInfinity;
    private float alertEndTime;

    public Animator Animator => animator;
    public CultistWeapon Weapon => weapon;
    public bool IsAlerted => Time.time < alertEndTime;
    public PoolTypeEnums CultistType => cultistType;

    private void Awake()
    {
        SetWeapon(weaponType);
    }

    public void SetUp(Transform player, CampFire fire)
    {
        target = player;
        campFire = fire;

        currentHp = maxHP;
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
        alertEndTime = Time.time + alertDuration;
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
        return distance <= scanRange * scanRange;
    }

    public void Chase()
    {
        if (target == null) return;

        navMesh.isStopped = false;
        navMesh.SetDestination(target.position);
    }

    public void ReturnToCamp()
    {
        navMesh.isStopped = false;

        Vector3 direction = (transform.position - campFire.transform.position).normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        Vector3 returnPosition = campFire.transform.position + direction * returnDistance;

        if (NavMesh.SamplePosition(returnPosition, out var hit, returnSearchRange, NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
    }

    public bool CheckArrive()
    {
        if (navMesh.pathPending || !navMesh.hasPath) return false;
        return navMesh.remainingDistance <= navMesh.stoppingDistance + 0.1f;
    }

    public bool IsAwayFromCamp()
    {
        if (campFire == null) return false;

        float distance = (campFire.transform.position - transform.position).sqrMagnitude;
        return distance > maxCampDistance * maxCampDistance;
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

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || currentHp <= 0) return;

        currentHp = Mathf.Max(currentHp - dmg, 0);
        if (currentHp <= 0)
        {
            cultistStateMachine.ChangeState(CultistStateEnums.DEAD);
            return;
        }

        Alert();
        cultistStateMachine.ChangeState(CultistStateEnums.CHASE);
    }
}

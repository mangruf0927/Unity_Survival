using UnityEngine;
using UnityEngine.AI;

public class CultistController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private CampFire campFire;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;

    // >>
    [SerializeField] private float scanRange;          // 플레이어 감지 범위
    [SerializeField] private float maxCampDistance;    // 플레이어를 놓쳤을 때 복귀를 판단하는 캠프 기준 거리
    [SerializeField] private float returnDistance;     // 캠프파이어 중심에서 떨어져 멈출 거리
    [SerializeField] private float returnSearchRange;  // 복귀 위치 주변에서 NavMesh 지점을 찾는 범위
    // <<

    public Animator Animator => animator;

    public void Stop()
    {
        navMesh.isStopped = true;
        navMesh.velocity = Vector3.zero;
        navMesh.ResetPath();
        animator.SetFloat("speed", 0f);
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
}

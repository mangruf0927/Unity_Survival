using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;

    private Transform target;

    public Transform Target => target;
    public Animator Animator => animator;
    public bool CanMove => navMesh != null && navMesh.enabled && navMesh.isOnNavMesh;

    private void Awake()
    {
        if (navMesh == null) navMesh = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Stop()
    {
        if (CanMove)
        {
            navMesh.isStopped = true;
            navMesh.ResetPath();
            navMesh.velocity = Vector3.zero;
        }

        if (animator != null) animator.SetFloat("speed", 0f);
    }

    public bool CheckRange(float range)
    {
        return CheckRange(target, range);
    }

    public bool CheckRange(Transform other, float range)
    {
        if (other == null || range < 0f) return false;
        float distance = (other.position - transform.position).sqrMagnitude;
        return distance <= range * range;
    }

    public bool Chase()
    {
        if (target == null) return false;
        return MoveTo(target.position, 2f);
    }

    public void Patrol(float patrolRange)
    {
        if (!CanMove || patrolRange <= 0f) return;

        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRange;
            Vector3 randomPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (MoveTo(randomPosition, 2f)) return;
        }
    }

    public bool MoveTo(Vector3 position, float searchRange)
    {
        if (!CanMove) return false;
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, searchRange, navMesh.areaMask)) return false;

        navMesh.isStopped = false;
        return navMesh.SetDestination(hit.position);
    }

    public bool CheckArrive()
    {
        if (!CanMove) return false;
        if (navMesh.pathPending || !navMesh.hasPath) return false;
        return navMesh.remainingDistance <= navMesh.stoppingDistance + 0.1f;
    }
}

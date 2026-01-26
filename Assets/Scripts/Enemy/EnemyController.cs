using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent navMesh;
    
    [SerializeField] private float scanRange;
    [SerializeField] private float patrolRange;

    public void Stop()
    {
        navMesh.isStopped = true;
        navMesh.ResetPath();
        navMesh.velocity = Vector3.zero;
    }

    public bool RangeCheck()
    {
        float distance = (target.position - transform.position).sqrMagnitude;
        return distance <= scanRange * scanRange;
    }

    public void Chase()
    {
        navMesh.isStopped = false;
        navMesh.SetDestination(target.position);
    }

    public void Patrol()
    {
        navMesh.isStopped = false;

        Vector3 random = Random.insideUnitSphere * patrolRange;
        Vector3 sourcePosition = transform.position + random;

        if (NavMesh.SamplePosition(sourcePosition, out var hit, patrolRange, NavMesh.AllAreas))
            navMesh.SetDestination(hit.position);
    }

    public float RandomTime()
    {
        return Random.Range(0f, 10f);
    }

    public bool CheckArrive()
    {
        if (navMesh.pathPending || navMesh.remainingDistance > 0f) return false;
        // if (navMesh.hasPath && navMesh.velocity.sqrMagnitude > 0.01f) return false;

        return true;
    }
}

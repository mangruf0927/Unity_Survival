using UnityEngine;
using UnityEngine.AI;

public class CultistController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;

    public Animator Animator => animator;

    public void Stop()
    {
        navMesh.isStopped = true;
        navMesh.ResetPath();
        navMesh.velocity = Vector3.zero;
    }
}

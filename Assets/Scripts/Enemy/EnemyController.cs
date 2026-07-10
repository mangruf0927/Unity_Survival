using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyDropper enemyDropper;

    private float alertEndTime;
    public bool IsAlerted => Time.time < alertEndTime;

    public PoolTypeEnums EnemyType => enemyStats.EnemyType;
    public Animator Animator => animator;

    public int EnemyId => enemyStats.EnemyId;
    public int CurrentHp => enemyStats.CurrentHp;

    private void Awake()
    {
        if (enemyDropper == null)
            enemyDropper = GetComponent<EnemyDropper>();
    }

    private void OnEnable()
    {
        if (enemyStats == null) return;
        enemyStats.OnDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        if (enemyStats == null) return;
        enemyStats.OnDamaged -= OnDamaged;
        alertEndTime = 0f;
    }

    public EnemySaveData CreateSaveData()
    {
        Vector3 position = transform.position;

        return new EnemySaveData
        {
            enemyId = EnemyId,
            positionX = position.x,
            positionY = position.y,
            positionZ = position.z,
            rotationY = transform.eulerAngles.y,
            currentHp = CurrentHp
        };
    }

    public void LoadSaveData(EnemySaveData saveData, EnemyData data)
    {
        enemyStats.SetUp(data);

        Vector3 position = new(saveData.positionX, saveData.positionY, saveData.positionZ);
        Quaternion rotation = Quaternion.Euler(0f, saveData.rotationY, 0f);

        transform.SetPositionAndRotation(position, rotation);

        enemyStats.LoadHp(saveData.currentHp);
    }

    private void OnDamaged(EnemyStats stats)
    {
        if (stats.CurrentHp <= 0) return;
        if (!stats.CanChase) return;
        Alert();
    }

    public void Stop()
    {
        navMesh.isStopped = true;
        navMesh.ResetPath();
        navMesh.velocity = Vector3.zero;
        animator.SetFloat("speed", 0f);
    }

    public void Alert()
    {
        alertEndTime = Time.time + enemyStats.AlertDuration;
    }

    public bool ShouldChasePlayer()
    {
        if (target == null) return false;
        if (!enemyStats.CanChase) return false;

        return IsAlerted || CheckRange();
    }

    public bool CheckRange()
    {
        if (target == null) return false;

        float distance = (target.position - transform.position).sqrMagnitude;
        return distance <= enemyStats.ScanRange * enemyStats.ScanRange;
    }

    public void Chase()
    {
        navMesh.isStopped = false;
        navMesh.SetDestination(target.position);
    }

    public void Patrol()
    {
        navMesh.isStopped = false;

        Vector3 randomPoint = transform.position + Random.insideUnitSphere * enemyStats.PatrolRange;

        if (NavMesh.SamplePosition(randomPoint, out var hit, enemyStats.PatrolRange, NavMesh.AllAreas))
            navMesh.SetDestination(hit.position);
    }

    public float RandomTime()
    {
        return Random.Range(0f, 10f);
    }

    public bool CheckArrive()
    {
        if (navMesh.pathPending || !navMesh.hasPath) return false;
        return navMesh.remainingDistance <= 0.1f;
    }

    public void DropItems()
    {
        enemyDropper.DropItems();
    }

    void OnTriggerEnter(Collider other)
    {
        if (enemyStats.AttackDamage <= 0 || !other.CompareTag("Player")) return;

        if (!other.TryGetComponent<IDamageable>(out var player)) return;
        player.TakeDamage(enemyStats.AttackDamage);
    }
}

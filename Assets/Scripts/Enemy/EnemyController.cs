using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyStats enemyStats;

    private Animator animator;
    private EnemyDropper enemyDropper;
    private EnemyStateMachine enemyStateMachine;

    private float alertEndTime;
    public bool IsAlerted => Time.time < alertEndTime;

    public PoolTypeEnums EnemyType => enemyStats.EnemyType;
    public Animator Animator => animator;

    public int EnemyId => enemyStats.EnemyId;
    public int CurrentHp => enemyStats.CurrentHp;

    private void Awake()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();

        enemyDropper = GetComponent<EnemyDropper>();
        enemyStateMachine = GetComponent<EnemyStateMachine>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (enemyStats == null) return;
        enemyStats.OnDamaged += OnDamaged;
        enemyStats.OnDead += OnDead;
    }

    private void OnDisable()
    {
        if (enemyStats == null) return;
        enemyStats.OnDamaged -= OnDamaged;
        enemyStats.OnDead -= OnDead;
        alertEndTime = 0f;
    }

    private void OnDamaged(EnemyStatsBase stats)
    {
        if (!enemyStats.CanChase) return;
        Alert();
    }

    private void OnDead(EnemyStatsBase stats)
    {
        enemyStateMachine.ChangeState(EnemyStateEnums.DEAD);
    }

    public void ResetState()
    {
        if (enemyStateMachine == null)
            enemyStateMachine = GetComponent<EnemyStateMachine>();

        enemyStateMachine?.InitializeState();
    }

    public void SetTarget(Transform target)
    {
        movement.SetTarget(target);
    }

    public void Stop()
    {
        movement.Stop();
    }

    public void Alert()
    {
        alertEndTime = Time.time + enemyStats.AlertDuration;
    }

    public bool ShouldChasePlayer()
    {
        if (movement.Target == null) return false;
        if (!enemyStats.CanChase) return false;

        return IsAlerted || CheckRange();
    }

    public bool CheckRange()
    {
        return movement.CheckRange(enemyStats.ScanRange);
    }

    public void Chase()
    {
        movement.Chase();
    }

    public void Patrol()
    {
        movement.Patrol(enemyStats.PatrolRange);
    }

    public float RandomTime()
    {
        return Random.Range(0f, 10f);
    }

    public bool CheckArrive()
    {
        return movement.CheckArrive();
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

    // Save/Load
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
}

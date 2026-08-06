using System.Threading;
using Cysharp.Threading.Tasks;

public class EnemyChaseState : IEnemyState
{
    private readonly EnemyController enemyController;
    private readonly EnemyStateMachine stateMachine;

    private CancellationTokenSource cts;

    public EnemyChaseState(EnemyStateMachine _stateMachine, EnemyController _enemyController)
    {
        enemyController = _enemyController;
        stateMachine = _stateMachine;
    }

    public void Enter()
    {
        enemyController.Animator.SetFloat("speed", 2f);
        cts = CancellationTokenSource.CreateLinkedTokenSource(stateMachine.destroyCancellationToken);
        RepathAsync(cts.Token).Forget();
    }

    private async UniTask RepathAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            enemyController.Chase();

            bool canceled = await UniTask.Delay(200, cancellationToken: ct).SuppressCancellationThrow();

            if (canceled) return;
        }
    }

    public void Update()
    {
        if (!enemyController.ShouldChasePlayer())
        {
            stateMachine.ChangeState(EnemyStateEnums.IDLE);
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }
}

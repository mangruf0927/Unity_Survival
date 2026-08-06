using System.Threading;
using Cysharp.Threading.Tasks;

public class CultistChaseState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    private CancellationTokenSource cts;

    public CultistChaseState(CultistStateMachine _stateMachine, CultistController _cultistController)
    {
        cultistStateMachine = _stateMachine;
        cultistController = _cultistController;
    }

    public void Enter()
    {
        cultistController.Animator.SetFloat("speed", 1f);
        cts = CancellationTokenSource.CreateLinkedTokenSource(cultistStateMachine.destroyCancellationToken);
        RepathAsync(cts.Token).Forget();
    }

    private async UniTask RepathAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            cultistController.Chase();

            bool canceled = await UniTask.Delay(200, cancellationToken: ct).SuppressCancellationThrow();

            if (canceled) return;
        }
    }

    public void Update()
    {
        if (!cultistController.ShouldChasePlayer())
        {
            if (cultistController.IsAwayFromRaidCenter())
            {
                cultistStateMachine.ChangeState(CultistStateEnums.RETURN);
            }
            else
            {
                cultistStateMachine.ChangeState(CultistStateEnums.IDLE);
            }
            return;
        }

        if (cultistController.CheckAttackRange() && cultistController.CanAttack())
        {
            cultistStateMachine.ChangeState(CultistStateEnums.ATTACK);
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

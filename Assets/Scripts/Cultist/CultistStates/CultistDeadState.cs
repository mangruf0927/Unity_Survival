public class CultistDeadState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    public CultistDeadState(CultistStateMachine _stateMachine, CultistController _cultistController)
    {
        cultistStateMachine = _stateMachine;
        cultistController = _cultistController;
    }

    public void Enter()
    {
        cultistController.Stop();
        cultistController.Animator.SetTrigger("Dead");
    }

    public void Update()
    {
        var state = cultistController.Animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("die") || state.normalizedTime < 1f) return;

        ObjectPool.Instance.ReturnToPool(cultistController.gameObject, cultistController.CultistType);
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}

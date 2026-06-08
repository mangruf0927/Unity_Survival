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
        cultistController.Animator.SetTrigger("Dead");
        cultistController.Stop();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}

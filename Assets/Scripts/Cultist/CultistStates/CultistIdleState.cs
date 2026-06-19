public class CultistIdleState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    public CultistIdleState(CultistStateMachine _stateMachine, CultistController _cultistController)
    {
        cultistStateMachine = _stateMachine;
        cultistController = _cultistController;
    }

    public void Enter()
    {
        cultistController.Stop();
        cultistController.Animator.SetFloat("speed", 0f);
    }

    public void Update()
    {
        if (cultistController.ShouldChasePlayer())
        {
            cultistStateMachine.ChangeState(CultistStateEnums.CHASE);
            return;
        }

        if (cultistController.IsAwayFromCamp())
        {
            cultistStateMachine.ChangeState(CultistStateEnums.RETURN);
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}

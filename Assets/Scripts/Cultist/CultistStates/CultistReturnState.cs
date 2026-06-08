public class CultistReturnState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    public CultistReturnState(CultistStateMachine _stateMachine, CultistController _cultistController)
    {
        cultistStateMachine = _stateMachine;
        cultistController = _cultistController;
    }

    public void Enter()
    {
        cultistController.Animator.SetFloat("speed", 1f);
        cultistController.ReturnToCamp();
    }

    public void Update()
    {
        if (cultistController.CheckArrive())
        {
            cultistStateMachine.ChangeState(CultistStateEnums.IDLE);
            return;
        }

        if (cultistController.CheckRange())
        {
            cultistStateMachine.ChangeState(CultistStateEnums.CHASE);
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}

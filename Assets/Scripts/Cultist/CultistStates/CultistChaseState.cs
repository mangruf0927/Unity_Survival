public class CultistChaseState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    public CultistChaseState(CultistStateMachine _stateMachine, CultistController _cultistController)
    {
        cultistStateMachine = _stateMachine;
        cultistController = _cultistController;
    }

    public void Enter()
    {
        cultistController.Animator.SetFloat("speed", 1f);
    }

    public void Update()
    {
        if (!cultistController.CheckRange())
        {
            if (cultistController.IsAwayFromCamp())
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
            return;
        }

        cultistController.Chase();
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}

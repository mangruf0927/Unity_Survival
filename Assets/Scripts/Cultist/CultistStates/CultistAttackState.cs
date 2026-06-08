public class CultistAttackState : ICultistState
{
    private readonly CultistStateMachine cultistStateMachine;
    private readonly CultistController cultistController;

    public CultistAttackState(CultistStateMachine _stateMachine, CultistController _cultistController)
    {
        cultistStateMachine = _stateMachine;
        cultistController = _cultistController;
    }

    public void Enter()
    {
        cultistController.Animator.SetTrigger("MeleeAttack");
        cultistController.Attack();
    }

    public void Update()
    {
        var state = cultistController.Animator.GetCurrentAnimatorStateInfo(1);

        if (state.IsName("attack_melee") && state.normalizedTime >= 1.0f)
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

            cultistStateMachine.ChangeState(CultistStateEnums.CHASE);
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
        cultistController.EndAttack();
    }
}

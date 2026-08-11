using UnityEngine;

public class CultistStateMachine : StateMachine<CultistStateEnums, ICultistState>
{
    [SerializeField] private CultistController cultistController;

    protected override CultistStateEnums InitialState => CultistStateEnums.IDLE;

    protected override void InitializeStates()
    {
        AddState(CultistStateEnums.IDLE, new CultistIdleState(this, cultistController));
        AddState(CultistStateEnums.CHASE, new CultistChaseState(this, cultistController));
        AddState(CultistStateEnums.ATTACK, new CultistAttackState(this, cultistController));
        AddState(CultistStateEnums.RETURN, new CultistReturnState(this, cultistController));
        AddState(CultistStateEnums.DEAD, new CultistDeadState(this, cultistController));
    }
}

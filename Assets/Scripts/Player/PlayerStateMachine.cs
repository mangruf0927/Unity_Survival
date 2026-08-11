using UnityEngine;

public class PlayerStateMachine : StateMachine<PlayerStateEnums, IPlayerState>
{
    [SerializeField] private PlayerController playerController;
    protected override PlayerStateEnums InitialState => PlayerStateEnums.IDLE;

    protected override void InitializeStates()
    {
        AddState(PlayerStateEnums.IDLE, new PlayerIdleState(this, playerController));
        AddState(PlayerStateEnums.MOVE, new PlayerMoveState(this, playerController));
        AddState(PlayerStateEnums.RUN, new PlayerRunState(this, playerController));
        AddState(PlayerStateEnums.JUMP, new PlayerJumpState(this, playerController));
        AddState(PlayerStateEnums.FALL, new PlayerFallState(this, playerController));
        AddState(PlayerStateEnums.ATTACK, new PlayerAttackState(this, playerController));
        AddState(PlayerStateEnums.DEAD, new PlayerDeadState(this, playerController));

    }

    public void ChangeInputState(PlayerStateEnums newStateType)
    {
        if (CurState == null) return;
        if (!CurState.InputHash.Contains(newStateType)) return;

        ChangeState(newStateType);
    }

    public void ChangeLogicState(PlayerStateEnums newStateType)
    {
        if (CurState == null) return;
        if (!CurState.LogicHash.Contains(newStateType)) return;

        ChangeState(newStateType);
    }
}

using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnums{ IDLE, MOVE, RUN, JUMP, ATTACK, DEAD };

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    public IPlayerState CurState {get; private set;}
    private Dictionary<PlayerStateEnums, IPlayerState> stateDictionary;

    private void Awake()
    {
        stateDictionary = new Dictionary<PlayerStateEnums, IPlayerState>()
        {
            {PlayerStateEnums.IDLE, new PlayerIdleState(this, playerController)},
            {PlayerStateEnums.MOVE, new PlayerMoveState(this, playerController)},
            {PlayerStateEnums.RUN, new PlayerRunState(this, playerController)},
            {PlayerStateEnums.JUMP, new PlayerJumpState(this, playerController)},
            {PlayerStateEnums.ATTACK, new PlayerAttackState(this, playerController)},
            {PlayerStateEnums.DEAD, new PlayerDeadState(this, playerController)},
        };

        if(stateDictionary.TryGetValue(PlayerStateEnums.IDLE, out IPlayerState newState))
        {
            CurState = newState;
            CurState.Enter();
        }
    }

    private void Update()
    {
        if (CurState != null) 
            CurState.Update();

        // Debug.Log(curState);
    }

    private void FixedUpdate()
    {
        if (CurState != null) 
            CurState.FixedUpdate();
    }

    public void ChangeInputState(PlayerStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IPlayerState newState)) return;
        if(CurState == newState) return;

        if(!CurState.InputHash.Contains(newStateType)) return;

        CurState?.Exit();
        CurState = newState;
        CurState.Enter();
    }

    public void ChangeLogicState(PlayerStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IPlayerState newState)) return;
        if(CurState == newState) return;

        if(!CurState.LogicHash.Contains(newStateType)) return;

        CurState?.Exit();
        CurState = newState;
        CurState.Enter();
    }

    public void ChangeState(PlayerStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IPlayerState newState)) return;
        if(CurState == newState) return;

        CurState?.Exit();
        CurState = newState;
        CurState.Enter();
    }
}

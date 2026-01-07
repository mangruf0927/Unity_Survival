using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnums{ IDLE, MOVE, JUMP };

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    public IPlayerState curState {get; private set;}
    private Dictionary<PlayerStateEnums, IPlayerState> stateDictionary;

    void Awake()
    {
        stateDictionary = new Dictionary<PlayerStateEnums, IPlayerState>()
        {
            {PlayerStateEnums.IDLE, new PlayerIdleState(this, playerController)},
            {PlayerStateEnums.MOVE, new PlayerMoveState(this, playerController)},
            {PlayerStateEnums.JUMP, new PlayerJumpState(this, playerController)},
        };

        if(stateDictionary.TryGetValue(PlayerStateEnums.IDLE, out IPlayerState newState))
        {
            curState = newState;
            curState.Enter();
        }
    }

    void Update()
    {
        if (curState != null) 
            curState.Update();

        // Debug.Log(curState);
    }

    void FixedUpdate()
    {
        if (curState != null) 
            curState.FixedUpdate();
    }

    public void ChangeInputState(PlayerStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IPlayerState newState)) return;
        if(curState == newState) return;

        if(!curState.inputHash.Contains(newStateType)) return;

        curState?.Exit();
        curState = newState;
        curState.Enter();
    }

    public void ChangeLogicState(PlayerStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IPlayerState newState)) return;
        if(curState == newState) return;

        if(!curState.logicHash.Contains(newStateType)) return;

        curState?.Exit();
        curState = newState;
        curState.Enter();
    }

    public void ChangeState(PlayerStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IPlayerState newState)) return;
        if(curState == newState) return;

        curState?.Exit();
        curState = newState;
        curState.Enter();
    }
}

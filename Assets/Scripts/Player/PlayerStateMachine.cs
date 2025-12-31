using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnums{ IDLE, MOVE, RUN }

public class PlayerStateMachine : MonoBehaviour
{    
    [SerializeField] private PlayerController playerController;

    public IPlayerState curState{ get; private set; }
    private Dictionary<PlayerStateEnums, IPlayerState> stateDictionary;

    private void Start()
    {
        stateDictionary = new Dictionary<PlayerStateEnums, IPlayerState>
        {
            { PlayerStateEnums.IDLE, new PlayerIdleState(this, playerController) },
            { PlayerStateEnums.MOVE, new PlayerMoveState(this, playerController) },
            { PlayerStateEnums.RUN, new PlayerRunState(this, playerController) }
        };

        if(stateDictionary.TryGetValue(PlayerStateEnums.IDLE, out IPlayerState newState))
        {
            curState = newState;
            curState.Enter();
        }
    }

    void Update()
    {
        if(curState != null) 
            curState.Update();
    }

    void FixedUpdate()
    {
        if(curState != null) 
            curState.FixedUpdate();
    }

    public void ChangeState(PlayerStateEnums newStateType)
    {
        if (!stateDictionary.TryGetValue(newStateType, out IPlayerState newState) || curState == null) return;

        curState?.Exit();        
        curState = newState;
        curState.Enter();
    }

    public bool CheckCurState(PlayerStateEnums stateType)
    {
        if(stateDictionary.TryGetValue(stateType, out IPlayerState stateValue))
        {
            return curState == stateValue;
        }
        return false;
    }
}
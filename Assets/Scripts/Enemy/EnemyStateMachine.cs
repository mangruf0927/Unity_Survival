using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateEnums { IDLE, CHASE, PATROL, DEAD };

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    
    public IEnemyState CurState {get; private set;}
    private Dictionary<EnemyStateEnums, IEnemyState> stateDictionary;

    private void Awake()
    {
        stateDictionary = new Dictionary<EnemyStateEnums, IEnemyState>()
        {
            {EnemyStateEnums.IDLE, new EnemyIdleState(this, enemyController)},
            {EnemyStateEnums.CHASE, new EnemyChaseState(this, enemyController)}, 
            {EnemyStateEnums.PATROL, new EnemyPatrolState(this, enemyController)},
            {EnemyStateEnums.DEAD, new EnemyDeadState(this, enemyController)},
        };

        if(stateDictionary.TryGetValue(EnemyStateEnums.IDLE, out IEnemyState newState))
        {
            CurState = newState;
            CurState.Enter();
        }
    }

    private void Update()
    {
        if(CurState != null) 
            CurState.Update();
        
        // Debug.Log(curState);
    }

    private void FixedUpdate()
    {
        if(CurState != null) 
            CurState.FixedUpdate();
    }

    public void ChangeState(EnemyStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IEnemyState newState)) return;
        if(CurState == newState) return;

        CurState?.Exit();
        CurState = newState;
        CurState.Enter();
    }
}

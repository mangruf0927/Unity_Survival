using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateEnums { IDLE, CHASE, PATROL };

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    
    public IEnemyState curState {get; private set;}
    private Dictionary<EnemyStateEnums, IEnemyState> stateDictionary;

    void Awake()
    {
        stateDictionary = new Dictionary<EnemyStateEnums, IEnemyState>()
        {
            {EnemyStateEnums.IDLE, new EnemyIdleState(this, enemyController)},
            {EnemyStateEnums.CHASE, new EnemyChaseState(this, enemyController)}, 
            {EnemyStateEnums.PATROL, new EnemyPatrolState(this, enemyController)},
        };

        if(stateDictionary.TryGetValue(EnemyStateEnums.IDLE, out IEnemyState newState))
        {
            curState = newState;
            curState.Enter();
        }
    }

    void Update()
    {
        if(curState != null) 
            curState.Update();
        
        // Debug.Log(curState);
    }

    void FixedUpdate()
    {
        if(curState != null) 
            curState.FixedUpdate();
    }

    public void ChangeState(EnemyStateEnums newStateType)
    {
        if(!stateDictionary.TryGetValue(newStateType, out IEnemyState newState)) return;
        if(curState == newState) return;

        curState?.Exit();
        curState = newState;
        curState.Enter();
    }
}

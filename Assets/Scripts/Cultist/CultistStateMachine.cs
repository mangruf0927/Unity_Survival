using System.Collections.Generic;
using UnityEngine;

public class CultistStateMachine : MonoBehaviour
{
    [SerializeField] private CultistController cultistController;

    public ICultistState CurState { get; private set; }
    private Dictionary<CultistStateEnums, ICultistState> stateDictionary;

    private void Awake()
    {
        stateDictionary = new Dictionary<CultistStateEnums, ICultistState>()
        {
            {CultistStateEnums.IDLE, new CultistIdleState(this, cultistController)},
            {CultistStateEnums.CHASE, new CultistChaseState(this, cultistController)},
            {CultistStateEnums.RETURN, new CultistReturnState(this, cultistController)},
            {CultistStateEnums.DEAD, new CultistDeadState(this, cultistController)},
        };
    }

    private void Start()
    {
        if (stateDictionary.TryGetValue(CultistStateEnums.IDLE, out ICultistState newState))
        {
            CurState = newState;
            CurState.Enter();
        }
    }

    private void Update()
    {
        if (CurState != null)
            CurState.Update();

        // Debug.Log(CurState);
    }

    private void FixedUpdate()
    {
        if (CurState != null)
            CurState.FixedUpdate();
    }

    public void ChangeState(CultistStateEnums newStateType)
    {
        if (!stateDictionary.TryGetValue(newStateType, out ICultistState newState)) return;
        if (CurState == newState) return;

        CurState?.Exit();
        CurState = newState;
        CurState.Enter();
    }
}

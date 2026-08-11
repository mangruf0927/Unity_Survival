using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class StateMachine<TStateEnums, TState> : MonoBehaviour
    where TStateEnums : struct, Enum
    where TState : class, IState
{
    protected abstract TStateEnums InitialState { get; }

    public TState CurState { get; private set; }
    protected readonly Dictionary<TStateEnums, TState> stateDictionary = new();

    protected void Awake()
    {
        InitializeStates();
    }

    protected void Start()
    {
        ChangeState(InitialState);
    }

    protected virtual void Update()
    {
        if (CurState != null)
            CurState.Update();

        // Debug.Log(CurState);
    }

    protected virtual void FixedUpdate()
    {
        if (CurState != null)
            CurState.FixedUpdate();
    }

    protected abstract void InitializeStates();

    protected void AddState(TStateEnums stateType, TState state)
    {
        stateDictionary.Add(stateType, state);
    }

    public void ChangeState(TStateEnums newStateType)
    {
        if (!stateDictionary.TryGetValue(newStateType, out TState newState)) return;
        if (CurState == newState) return;

        CurState?.Exit();
        CurState = newState;
        CurState.Enter();
    }

    public void InitializeState()
    {
        CurState?.Exit();
        CurState = null;

        ChangeState(InitialState);
    }
}

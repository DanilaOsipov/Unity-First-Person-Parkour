using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State CurrentState { get; private set; }

    public void Initialize(State startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(State newState, object obj = null)
    {
        CurrentState.Exit();

        CurrentState = newState;
        newState.Enter(obj);
    }
}

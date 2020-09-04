using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected Character character;
    protected StateMachine stateMachine;

    public State(Character character, StateMachine stateMachine)
    {
        this.character = character;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter(object obj = null);

    public abstract void HandleInput();

    public abstract void LogicUpdate();

    public abstract void PhysicsUpdate();

    public abstract void Exit();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DodgingState : HorizontalGroundedMovingState
{
    private float dodgingTime;

    public DodgingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        base.Enter();

        speed = character.DodgingSpeed;
        dodgingTime = 0;
    }

    public override void Exit()
    {
       
    }

    public override void HandleInput()
    {
      
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        dodgingTime += Time.deltaTime;

        if (dodgingTime >= character.DodgingTime)
        {
            stateMachine.ChangeState(character.Standing);
        }
        else
        {
            Vector3 movement = character.transform.TransformDirection(Vector3.back * speed);
            movement *= Time.deltaTime;
            character.CharacterController.Move(movement);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}


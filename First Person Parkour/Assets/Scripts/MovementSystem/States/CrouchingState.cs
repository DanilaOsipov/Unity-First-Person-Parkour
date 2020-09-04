using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CrouchingState : HorizontalGroundedMovingState
{
    private bool stand, belowCeiling;

    public CrouchingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        base.Enter();

        character.SetCrouchingHeight();
        speed = character.CrouchingSpeed;
        stand = belowCeiling = false;
    }

    public override void Exit()
    {
        character.SetStandingHeight();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        stand = Input.GetButtonDown("Fire3");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!belowCeiling && stand)
            stateMachine.ChangeState(character.Standing);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        belowCeiling =  Physics.CheckSphere(character.transform.position + Vector3.up * character.CrouchingColliderHeight,
                                            character.CharacterController.radius);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingState : HorizontalGroundedMovingState
{
    private float slidingTime;
    private bool belowCeiling;

    public SlidingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        base.Enter();

        character.SetCrouchingHeight();
        speed = character.SlidingSpeed;
        slidingTime = 0;
        belowCeiling = false;

        MouseLookManager.PausePlayerRotation();
        MouseLookManager.MainCamera.ClampYRotation(-30.0f, 30.0f);
    }

    public override void Exit()
    {
        MouseLookManager.ResumePlayerRotation();
        MouseLookManager.MainCamera.StopYRotationClamping();
    }

    public override void HandleInput()
    {
       
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        slidingTime += Time.deltaTime;

        if (slidingTime >= character.SlidingTime)
        {
            if (belowCeiling)
            {
                stateMachine.ChangeState(character.Crouching);
            }
            else
            {
                stateMachine.ChangeState(character.Standing);
            }
        }
        else
        {
            Vector3 movement = character.transform.TransformDirection(Vector3.forward * speed);
            movement *= Time.deltaTime;
            character.CharacterController.Move(movement);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        belowCeiling = Physics.CheckSphere(character.transform.position + Vector3.up * character.CrouchingColliderHeight,
                                           character.CharacterController.radius);
    }
}

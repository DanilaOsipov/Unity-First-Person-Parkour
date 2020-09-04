using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClimbingState : State
{
    private BoxCollider obstacle;
    private Vector3 destination;
    private bool belowCeiling;

    public ClimbingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        obstacle = obj as BoxCollider;
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
        if (character.transform.position.y < obstacle.bounds.max.y + character.CrouchingColliderHeight / 2)
        {
            character.CharacterController.Move(Vector3.up * character.ClimbingSpeed * Time.deltaTime);
            return;
        }

        if (character.CharacterController.height != character.CrouchingColliderHeight)
        {
            character.CharacterController.height = character.CrouchingColliderHeight;

            destination = character.transform.position + 2.5f * character.CharacterController.radius * character.transform.forward;
            return;
        }

        if ((destination - character.transform.position).sqrMagnitude > 0.09f)
        {
            character.CharacterController.Move(character.transform.forward * character.ClimbingSpeed * Time.deltaTime);
        }
        else
        {
            if (belowCeiling)
                stateMachine.ChangeState(character.Crouching);
            else
                stateMachine.ChangeState(character.Standing);
        }
    }

    public override void PhysicsUpdate()
    {
        belowCeiling = Physics.CheckSphere(character.transform.position + Vector3.up * character.CrouchingColliderHeight,
                                           character.CharacterController.radius);
    }
}


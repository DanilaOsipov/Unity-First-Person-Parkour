using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VaultingState : State
{
    private bool exit, startChecking;
    private Vector3 destination;

    public VaultingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        exit = startChecking = false;

        MouseLookManager.PausePlayerRotation();
        MouseLookManager.MainCamera.ClampYRotation(-30.0f, 30.0f);

        character.CharacterController.Move(Vector3.up * character.CharacterController.height * 0.25f);

        character.SetCrouchingHeight();

        destination = character.transform.position + 2 * character.CharacterController.radius * character.transform.forward;
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
        if (exit) 
            stateMachine.ChangeState(character.Standing);

        if (!startChecking)
            startChecking = (destination - character.transform.position).sqrMagnitude < 0.09f;

        Vector3 motion = character.transform.forward * character.VaultingSpeed * Time.deltaTime;

        character.CharacterController.Move(motion);
    }

    public override void PhysicsUpdate()
    {
        if (startChecking)
            exit = !Physics.CheckSphere(character.transform.position + Vector3.down * character.CrouchingColliderHeight,
                                        character.CharacterController.radius);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WallRunningState : State
{
    private Vector3 wallNormal;
    private bool? wallToLeft;
    private float gravity;
    private bool wallRun, jump, hitGround; 

    public WallRunningState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        gravity = character.WallRunningGravity;
        wallRun = jump = true;
        hitGround = false;

        MouseLookManager.PausePlayerRotation();
        MouseLookManager.MainCamera.ClampYRotation(-90.0f, 90.0f);

        object[] obs = (object[])obj;
        wallNormal = (Vector3)obs[0];
        wallToLeft = (bool?)obs[1];

        float angle = 0;
        if ((bool)wallToLeft)
        {
            angle = Vector3.SignedAngle(wallNormal, character.transform.right, Vector3.up);

            character.SetCameraZRotation(-45.0f);
        }
        else
        {
            angle = Vector3.SignedAngle(wallNormal, -character.transform.right, Vector3.up);

            character.SetCameraZRotation(45.0f);
        }

        Vector3 euler = character.transform.eulerAngles;
        euler.y -= angle;
        character.transform.eulerAngles = euler;

        euler = MouseLookManager.MainCamera.transform.localEulerAngles;
        euler.y += angle;
        MouseLookManager.MainCamera.transform.localEulerAngles = euler;
    }

    public override void Exit()
    {
        character.SetCameraZRotation(0);

        MouseLookManager.ResumePlayerRotation();
        MouseLookManager.MainCamera.StopYRotationClamping();
    }

    public override void HandleInput()
    {
        jump = Input.GetButtonDown("Jump");
    }

    public override void LogicUpdate()
    {
        if (wallRun)
        {
            if (jump) stateMachine.ChangeState(character.JumpingState);
            else
            {
                if (hitGround) stateMachine.ChangeState(character.Standing);

                character.CharacterController.Move(character.transform.forward * character.RunningSpeed * Time.deltaTime);

                Vector3 motion = Vector3.zero;
                motion.y = gravity;
                character.CharacterController.Move(motion * Time.deltaTime);
            }
        }
        else stateMachine.ChangeState(character.Standing);
    }

    public override void PhysicsUpdate()
    {
        wallRun = character.IsCloseToWall(out wallNormal, out wallToLeft);
        hitGround = Physics.Raycast(character.transform.position, Vector3.down, character.CharacterController.height / 2 + 0.1f);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingState : HorizontalGroundedMovingState
{
    private bool jump, wallRun;
    private float vertSpeed;
    private BoxCollider obstacle;
    private Vector3 wallNormal;
    private bool? wallToLeft;

    public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        base.Enter();

        speed = character.RunningSpeed;
        vertSpeed = character.JumpSpeed;
        jump = wallRun = false;
        obstacle = null;
        wallNormal = default;
        wallToLeft = null;

        gravity = 0;
    }

    public override void Exit()
    {
        
    }

    public override void HandleInput()
    {
        base.HandleInput();

        jump = Input.GetButtonDown("Jump");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (wallRun && jump && vertInput > 0 && horInput == 0)
        {
            stateMachine.ChangeState(character.WallRunning, new object [] { wallNormal, wallToLeft });
            return;
        }

        if (obstacle != null && jump)
        {
            if (obstacle.bounds.max.y - (character.transform.position.y - character.CharacterController.height / 2)
                <= character.JumpToClimbObstacleMaxHeight)
            {
                stateMachine.ChangeState(character.Climbing, obstacle);
                return;
            }
        }

        Vector3 movement = Vector3.zero;

        vertSpeed += character.Gravity * 5 * Time.deltaTime;
        if (vertSpeed < character.Gravity)
        {
            if (hitGround)
            {
                stateMachine.ChangeState(character.Standing);
                return;
            }

            vertSpeed = character.Gravity;
        }

        movement.y = vertSpeed;
        movement *= Time.deltaTime;

        character.CharacterController.Move(movement); 
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        character.IsCloseToClimbingObstacle(out obstacle);

        wallRun = character.IsCloseToWall(out wallNormal, out wallToLeft);
    }
}


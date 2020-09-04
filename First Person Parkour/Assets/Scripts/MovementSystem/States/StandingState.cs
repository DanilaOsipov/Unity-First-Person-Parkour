using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StandingState : HorizontalGroundedMovingState
{
    private bool crouch, jump, vault, climb;
    private BoxCollider obstacle;

    public StandingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {
        
    }

    public override void Enter(object obj = null)
    {
        base.Enter();

        character.SetStandingHeight();
        speed = character.RunningSpeed;
        crouch = jump = vault = climb = false;
        obstacle = null;
    }

    public override void Exit()
    {

    }

    public override void HandleInput()
    {
        base.HandleInput();

        crouch = Input.GetButtonDown("Fire3");
        jump = Input.GetButtonDown("Jump");
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (crouch)
        {
            if (hitGround)
            {
                if (vertInput == 1 && horInput == 0)
                    stateMachine.ChangeState(character.Sliding);
                else
                    stateMachine.ChangeState(character.Crouching);
            }
        }
        else if (jump)
        {
            if (vertInput < 0 && horInput == 0)
            {
                if (hitGround)
                {
                    stateMachine.ChangeState(character.Dodging);
                    return;
                }
            }
            else if (climb)
            {
                float dot = Vector3.Dot(character.transform.forward, obstacle.transform.forward);

                vault = obstacle.bounds.size.z <= character.VaultingObstacleMaxDepth &&
                        obstacle.bounds.max.y <= character.transform.position.y &&
                        character.transform.position.y <= obstacle.bounds.max.y + 0.1f &&
                        Mathf.Abs(dot) >= 0.9f;

                if (vertInput == 1 && horInput == 0 && vault)
                {
                    stateMachine.ChangeState(character.Vaulting);
                    return;
                }

                if (obstacle.bounds.max.y - (character.transform.position.y - character.CharacterController.height / 2)
                    <= character.StandToClimbObstacleMaxHeight)
                {
                    stateMachine.ChangeState(character.Climbing, obstacle);
                    return;
                }
            }

            if (hitGround) stateMachine.ChangeState(character.JumpingState);
        }       
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        climb = character.IsCloseToClimbingObstacle(out obstacle);
    }
}

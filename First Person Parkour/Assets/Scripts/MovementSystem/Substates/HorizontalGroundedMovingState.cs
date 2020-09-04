using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HorizontalGroundedMovingState : State
{
    protected float speed, vertInput, horInput, gravity;
    protected bool hitGround;

    public HorizontalGroundedMovingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter(object obj = null)
    {
        vertInput = horInput = 0;
        hitGround = false;
        gravity = character.Gravity;
    }

    public override void Exit()
    {
       
    }

    public override void HandleInput()
    {
        vertInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");
    }

    public override void LogicUpdate()
    {
        Vector3 movement = Vector3.zero;

        movement.x = horInput * speed;
        movement.z = vertInput * speed;
        movement = Vector3.ClampMagnitude(movement, speed);
        movement = character.transform.TransformDirection(movement);

        if (!hitGround)
        {
            if (character.CharacterController.isGrounded)
            {
                if (Vector3.Dot(movement, character.Contact.normal) < 0)
                {
                    movement = character.Contact.normal * character.RunningSpeed;
                }
                else
                {
                    movement += character.Contact.normal * character.RunningSpeed;
                }
            }
        }

        movement.y = gravity;
        movement *= Time.deltaTime;

        character.CharacterController.Move(movement);
    }

    public override void PhysicsUpdate()
    {
        hitGround = Physics.Raycast(character.transform.position, Vector3.down, character.CharacterController.height / 2 + 0.1f);
    }
}


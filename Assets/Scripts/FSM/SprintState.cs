using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : State
{
    bool sprint;
    bool sprintJump;
    bool isGrounded;
    float playerSpeed;
    float gravityValue;

    Vector3 currentVelocity;
    Vector3 smoothVelocity;

    public SprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        sprint = false;
        sprintJump = false;

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = character.sprintSpeed;
        gravityValue = character.GRAVITY_VALUE;
        isGrounded = character.controller.isGrounded;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;

        if (sprintAction.triggered || input.sqrMagnitude == 0f)
        {
            sprint = false;
        }
        else
        {
            sprint = true;
        }

        if (jumpAction.triggered)
        {
            sprintJump = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (sprint)
        {
            character.animator.SetFloat("speed", input.magnitude + 1f, character.speedDampTime, Time.deltaTime);
        }
        else
        {
            stateMachine.ChangeState(character.idleState);
        }
        if (sprintJump)
        {
            stateMachine.ChangeState(character.sprintJumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = character.controller.isGrounded;
        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref smoothVelocity, character.velocityDampTime);

        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }
}

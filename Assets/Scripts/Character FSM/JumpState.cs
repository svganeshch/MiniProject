using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    bool isGrounded;

    float gravityValue;
    float jumpHeight;
    float playerSpeed;

    Vector3 airVelocity;

    public JumpState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = false;
        gravityValue = character.GRAVITY_VALUE;
        jumpHeight = character.jumpHeight;
        playerSpeed = character.walkingSpeed;
        gravityVelocity.y = 0;

        character.animator.SetFloat("speedY", 0);
        character.animator.SetTrigger("jump");

        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isGrounded)
        {
            stateMachine.ChangeState(character.landState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isGrounded)
        {
            moveVelocity = character.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);

            moveVelocity = horizontalInput * PlayerCamera.instance.transform.right + moveVelocity.z * PlayerCamera.instance.transform.forward.normalized;
            moveVelocity.y = 0f;

            airVelocity = airVelocity.x * PlayerCamera.instance.transform.right.normalized + airVelocity.z * PlayerCamera.instance.transform.forward.normalized;
            airVelocity.y = 0f;

            character.controller.Move(gravityVelocity * Time.fixedDeltaTime + playerSpeed * Time.fixedDeltaTime * (airVelocity * character.airControl + moveVelocity * (1 - character.airControl)));
        }

        gravityVelocity.y += gravityValue * Time.fixedDeltaTime;
        isGrounded = character.controller.isGrounded;
    }

    private void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}

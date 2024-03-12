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
        moveVelocity = Vector3.zero;
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
        moveVelocity = new Vector3(input.x, 0, input.y);

        moveVelocity = moveVelocity.x * PlayerCamera.instance.transform.right.normalized + moveVelocity.z * PlayerCamera.instance.transform.forward.normalized;
        moveVelocity.y = 0f;

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

        if (moveAmount > 0.5f)
        {
            // running speed
            character.controller.Move(character.runningSpeed * Time.deltaTime * moveVelocity + gravityVelocity * Time.deltaTime);
        }
        else if (moveAmount <= 0.5f)
        {
            // walking speed
            character.controller.Move(character.walkingSpeed * Time.deltaTime * moveVelocity + gravityVelocity * Time.deltaTime);
        }

        HandleRotation();
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = PlayerCamera.instance.cameraObj.transform.forward * verticalInput;
        targetDirection += PlayerCamera.instance.cameraObj.transform.right * horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0f;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = character.transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetDirection);
        Quaternion targetRotation = Quaternion.Slerp(character.transform.rotation, newRotation, character.rotationDampTime * Time.deltaTime);
        character.transform.rotation = targetRotation;
    }
}

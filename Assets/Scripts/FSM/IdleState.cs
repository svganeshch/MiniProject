using System;
using UnityEngine;

public class IdleState : State
{
    bool jump;
    bool sprint;
    bool isGrounded;
    bool drawWeapon;

    int weaponSlot = 1;
    float gravityValue;

    public IdleState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        jump = false;
        sprint = false;
        drawWeapon = false;
        input = Vector2.zero;
        moveVelocity = Vector3.zero;
        targetDirection = Vector3.zero;
        gravityVelocity.y = 0;

        gravityValue = character.GRAVITY_VALUE;
        isGrounded = character.controller.isGrounded;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered)
            jump = true;

        if (sprintAction.triggered)
            sprint = true;

        if (dodgeAction.triggered)
        {
            if (character.animator.GetFloat("speedY") >= 0.5f)
                character.animator.SetTrigger("dodge");
        }

        if (drawWeaponAction.triggered)
            drawWeapon = true;

        if (weapon1Action.triggered)
        {
            weaponSlot = 1;
            drawWeapon = true;
        }
        if (weapon2Action.triggered)
        {
            weaponSlot = 2;
            drawWeapon = true;
        }
        if (weapon3Action.triggered)
        {
            weaponSlot = 3;
            drawWeapon = true;
        }
        if (weapon4Action.triggered)
        {
            weaponSlot = 4;
            drawWeapon = true;
        }

        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        moveVelocity = PlayerCamera.instance.transform.forward * verticalInput;
        moveVelocity += PlayerCamera.instance.transform.right * horizontalInput;
        moveVelocity.Normalize();
        moveVelocity.y = 0;

        character.animator.SetFloat("speedX", 0, character.speedDampTime, Time.deltaTime);
        character.animator.SetFloat("speedY", moveAmount, character.speedDampTime, Time.deltaTime);

        if (jump)
            stateMachine.ChangeState(character.jumpState);

        if (sprint)
            stateMachine.ChangeState(character.sprintState);

        if (drawWeapon)
        {
            character.weaponEquipment.SetWeapon(weaponSlot);
            character.animator.SetTrigger("drawWeapon");
            stateMachine.ChangeState(character.combatState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.fixedDeltaTime;
        isGrounded = character.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        if (moveAmount > 0.5f)
        {
            // running speed
            character.controller.Move(character.runningSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
        }
        else if (moveAmount <= 0.5f)
        {
            // walking speed
            character.controller.Move(character.walkingSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
        }

        HandleRotation();
    }

    private void HandleRotation()
    {
        targetDirection = Vector3.zero;
        targetDirection = PlayerCamera.instance.cameraObj.transform.forward * verticalInput;
        targetDirection += PlayerCamera.instance.cameraObj.transform.right * horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0f;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = character.transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetDirection);
        Quaternion targetRotation = Quaternion.Slerp(character.transform.rotation, newRotation, character.rotationDampTime * Time.fixedDeltaTime);
        character.transform.rotation = targetRotation;
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);
        character.transform.rotation = Quaternion.LookRotation(targetDirection);
    }
}

using System;
using UnityEngine;

public class IdleState : State
{
    bool jump;
    bool sprint;
    bool isGrounded;
    bool drawWeapon;

    int weaponSlot;
    int defaultWeaponSlot = 1;
    public int previousWeaponSlot = 0;
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

        if (previousWeaponSlot != 0)
        {
            weaponSlot = previousWeaponSlot;
        }
        else
        {
            weaponSlot = defaultWeaponSlot;
        }

        character.animator.SetBool("isCombat", false);
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
            stateMachine.ChangeState(character.dodgeState);
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

        SetAnimationParameters(0, moveAmount);

        if (jump)
            stateMachine.ChangeState(character.jumpState);

        if (sprint)
            stateMachine.ChangeState(character.sprintState);

        if (drawWeapon)
        {
            drawWeapon = false;
            if (character.weaponEquipment.SetWeapon(weaponSlot))
            {
                previousWeaponSlot = weaponSlot;

                character.animator.SetTrigger("drawWeapon");
                character.animator.SetBool("isWeaponDraw", true);
                //stateMachine.ChangeState(character.combatState);
            }
            else
            {
                weaponSlot = defaultWeaponSlot;
            }
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

    private void SetAnimationParameters(float horizontalInput, float verticalInput)
    {
        float snappedHorizontal = horizontalInput;
        float snappedVertical = verticalInput;

        if (horizontalInput > 0 && horizontalInput <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalInput > 0.5f && horizontalInput <= 1)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalInput < 0 && horizontalInput >= -0.5f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalInput < -0.5f && horizontalInput >= -1)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        if (verticalInput > 0 && verticalInput <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalInput > 0.5f && verticalInput <= 1)
        {
            snappedVertical = 1;
        }
        else if (verticalInput < 0 && verticalInput >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalInput < -0.5f && verticalInput >= -1)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        character.animator.SetFloat("speedX", snappedHorizontal, character.speedDampTime, Time.deltaTime);
        character.animator.SetFloat("speedY", snappedVertical, character.speedDampTime, Time.deltaTime);
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);
        character.transform.rotation = Quaternion.LookRotation(targetDirection);
    }
}

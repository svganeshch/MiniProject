using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.TextCore.Text;

public class CombatState : State
{
    public Enemy currentTarget;
    public bool isLockedOn = false;

    float gravityValue;
    float playerSpeed;

    int swapWeaponTo;

    bool isGrounded;
    bool holsterWeapon;
    bool attackState;
    bool heavyAttackState;
    bool swapWeapon;
    bool lockOnTrigger;
    bool leftLockOnTrigger;
    bool rightLockOnTrigger;

    Coroutine lockOnCoroutine;

    public CombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        holsterWeapon = false;
        attackState = false;
        heavyAttackState = false;
        swapWeapon = false;
        lockOnTrigger = false;
        leftLockOnTrigger = false;
        rightLockOnTrigger = false;

        input = Vector2.zero;
        targetDirection = Vector3.zero;
        gravityVelocity.y = 0;

        moveVelocity = character.playerVelocity;
        playerSpeed = character.combatSpeed;
        isGrounded = character.controller.isGrounded;
        gravityValue = character.GRAVITY_VALUE;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            stateMachine.ChangeState(character.dodgeState);
        }

        if (drawWeaponAction.triggered)
        {
            holsterWeapon = true;
        }

        if (heavyAttackWeaponAction.triggered)
        {
            heavyAttackState = true;
        }

        if (liteAttackWeaponAction.triggered && !heavyAttackState)
        {
            if (character.animator.GetBool("swapWeapon"))
                return;

            attackState = true;
        }

        if (weapon1Action.triggered)
        {
            swapWeaponTo = 1;
            swapWeapon = true;
        }
        if (weapon2Action.triggered)
        {
            swapWeaponTo = 2;
            swapWeapon = true;
        }
        if (weapon3Action.triggered)
        {
            swapWeaponTo = 3;
            swapWeapon = true;
        }
        if (weapon4Action.triggered)
        {
            swapWeaponTo = 4;
            swapWeapon = true;
        }

        if (lockOnAction.triggered)
        {
            lockOnTrigger = true;
        }

        // target lock on swap
        if (isLockedOn)
        {
            if (leftLockOnAction.triggered || rightLockOnAction.triggered)
            {
                if (character.playerInput.devices[0] is Gamepad)
                {
                    if (leftLockOnAction.triggered)
                    {
                        leftLockOnTrigger = true;
                    }

                    if (rightLockOnAction.triggered)
                    {
                        rightLockOnTrigger = true;
                    }
                }
                else if (character.playerInput.devices[0] is Keyboard)
                {
                    if (leftLockOnAction.ReadValue<float>() <= -PlayerCamera.instance.mouseLockOnSwitchTreshold)
                    {
                        leftLockOnTrigger = true;
                    }

                    if (rightLockOnAction.ReadValue<float>() >= PlayerCamera.instance.mouseLockOnSwitchTreshold)
                    {
                        rightLockOnTrigger = true;
                    }
                }
            }
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

        if (isLockedOn)
        {
            SetAnimationParameters(horizontalInput, moveAmount);
        }
        else
        {
            SetAnimationParameters(0, moveAmount);
        }

        if (holsterWeapon)
        {
            ResetLockOn();

            character.animator.SetTrigger("holsterWeapon");
            stateMachine.ChangeState(character.idleState);
        }

        if (heavyAttackState)
        {
            heavyAttackState = false;
            //character.animator.SetTrigger("heavyAttack");
            stateMachine.ChangeState(character.heavyAttackState);
        }

        if (attackState)
        {
            attackState = false;
            //character.animator.SetTrigger("attack");
            stateMachine.ChangeState(character.liteAttackState);
        }

        if (swapWeapon)
        {
            swapWeapon = false;
            HandleWeaponSwap();
        }

        if (isLockedOn)
        {
            CheckLockOn();
        }
        if (lockOnTrigger)
        {
            HandleTargetLockOn();
        }
        if (leftLockOnTrigger || rightLockOnTrigger)
        {
            HandleTargetLockOnSwitch();
        }
    }

    private void HandleWeaponSwap()
    {
        if (character.weaponEquipment.GetCurrentWeapon().weaponSlot == swapWeaponTo)
        {
            ResetLockOn();

            character.animator.SetTrigger("holsterWeapon");
            stateMachine.ChangeState(character.idleState);

            return;
        }

        if (character.weaponEquipment.GetWeaponWithSlot(swapWeaponTo).Enabled)
        {
            SlowDownTime();

            character.animator.SetFloat("holsterSpeed", character.weaponEquipment.weaponSwapSpeed);
            character.animator.SetFloat("drawSpeed", character.weaponEquipment.weaponSwapSpeed);

            character.animator.SetBool("swapWeapon", true);
            character.animator.SetTrigger("holsterWeapon");
        }
    }

    private void CheckLockOn()
    {
        if (currentTarget == null)
            return;

        if (currentTarget.isDead)
        {
            ResetLockOn();

            if (lockOnCoroutine != null)
                character.StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = character.StartCoroutine(PlayerCamera.instance.WaitFindNewTarget());
        }
    }

    private void HandleTargetLockOn()
    {
        if (lockOnTrigger && isLockedOn)
        {
            lockOnTrigger = false;

            PlayerCamera.instance.ClearLockOnTargets();
            SetTarget(null);
            isLockedOn = false;

            return;
        }

        if (lockOnTrigger && !isLockedOn)
        {
            lockOnTrigger = false;

            PlayerCamera.instance.FindLockOnTarget();

            if (PlayerCamera.instance.nearestTarget != null)
            {
                SetTarget(PlayerCamera.instance.nearestTarget);
                isLockedOn = true;
            }
        }
    }

    private void HandleTargetLockOnSwitch()
    {
        if (leftLockOnTrigger)
        {
            leftLockOnTrigger = false;

            if (isLockedOn)
            {
                PlayerCamera.instance.FindLockOnTarget();

                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }

        if (rightLockOnTrigger)
        {
            rightLockOnTrigger = false;

            if (isLockedOn)
            {
                PlayerCamera.instance.FindLockOnTarget();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    private void ResetLockOn()
    {
        isLockedOn = false;
        SetTarget(null);
    }

    public void SetTarget(Enemy nearestTarget)
    {
        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;
        }
        else
        {
            currentTarget = null;
        }

        PlayerCamera.instance.SetLockOnCameraHeight();
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
            character.controller.Move(playerSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
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
        if (isLockedOn)
        {
            if (currentTarget == null)
                return;

            Vector3 lockedTargetDirection;
            lockedTargetDirection = currentTarget.transform.position - character.transform.position;
            lockedTargetDirection.y = 0f;
            lockedTargetDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            Quaternion finalRotation = Quaternion.Slerp(character.transform.rotation, targetRotation, character.rotationDampTime * Time.fixedDeltaTime);
            character.transform.rotation = finalRotation;
        }
        else
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

        if (isLockedOn)
        {
            if (moveVelocity == Vector3.zero)
            {
                moveVelocity = character.transform.forward;
            }
            character.transform.rotation = Quaternion.LookRotation(moveVelocity);
        }
        else
        {
            character.transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }

    public void SwapWeapon()
    {
        if (character.weaponEquipment.SetWeapon(swapWeaponTo))
        {
            character.idleState.previousWeaponSlot = swapWeaponTo;
            character.animator.SetTrigger("drawWeapon");
        }
    }

    private void SlowDownTime()
    {
        float slowTime = character.weaponSwapSlowTime;

        Time.timeScale = slowTime;
        Time.fixedDeltaTime = slowTime * Time.deltaTime;
    }
}

using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CombatState : State
{
    Enemy currentTarget;
    bool isLockedOn = false;

    float gravityValue;
    float playerSpeed;

    int swapWeaponTo;

    bool isGrounded;
    bool holsterWeapon;
    bool attackState;
    bool heavyAttackState;
    bool swapWeapon;
    bool swapTrigger;
    bool lockOnTrigger;

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
        swapTrigger = false;
        lockOnTrigger = false;

        input = Vector2.zero;
        targetDirection = Vector3.zero;
        gravityVelocity.y = 0;

        moveVelocity = character.playerVelocity;
        playerSpeed = character.combatSpeed;
        isGrounded = character.controller.isGrounded;
        gravityValue = character.GRAVITY_VALUE;

        character.animator.SetBool("isCombat", true);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            if (character.animator.GetFloat("speedY") >= 0.5f)
                character.animator.SetTrigger("dodge");
        }

        if (drawWeaponAction.triggered)
        {
            holsterWeapon = true;
        }

        if (heavyAttackWeaponAction.triggered)
        {
            heavyAttackState = true;
        }

        if (attackWeaponAction.triggered && !heavyAttackState)
        {
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

        character.animator.SetFloat("speedY", input.magnitude, character.speedDampTime, Time.deltaTime);

        if (holsterWeapon)
        {
            character.animator.SetTrigger("holsterWeapon");
            stateMachine.ChangeState(character.idleState);
        }

        if (heavyAttackState)
        {
            character.animator.SetTrigger("heavyAttack");
            stateMachine.ChangeState(character.heavyAttackState);
        }

        if (attackState)
        {
            character.animator.SetTrigger("attack");
            stateMachine.ChangeState(character.attackState);
        }

        if (swapWeapon)
        {
            if (character.weaponEquipment.GetCurrentWeapon().weaponSlot == swapWeaponTo)
            {
                swapWeapon = false;
                character.animator.SetTrigger("holsterWeapon");
                stateMachine.ChangeState(character.idleState);
                return;
            }

            character.animator.SetFloat("holsterSpeed", 2);
            character.animator.SetFloat("drawSpeed", 2);

            character.animator.SetTrigger("holsterWeapon");

            swapTrigger = true;
            swapWeapon = false;
        }

        if (swapTrigger)
        {
            if (character.weaponEquipment.weaponHolsterDone)
            {
                if (character.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"))
                {
                    swapTrigger = false;
                    character.weaponEquipment.weaponHolsterDone = false;
                    character.StartCoroutine(SwapWeapon());
                }
            }
        }

        if (isLockedOn)
        {
            if (currentTarget == null)
                return;

            if (currentTarget.isDead)
            {
                ResetTargetLock();
                isLockedOn = false;
            }
        }
        if (lockOnTrigger)
        {
            HandleTargetLockOn();
        }
    }

    private void HandleTargetLockOn()
    {
        if (lockOnTrigger && isLockedOn)
        {
            lockOnTrigger = false;

            character.cameraTargetLock.ClearLockOnTargets();
            isLockedOn = false;

            ResetTargetLock();

            return;
        }

        if (lockOnTrigger && !isLockedOn)
        {
            lockOnTrigger = false;

            character.cameraTargetLock.FindLockOnTarget();

            if (character.cameraTargetLock.nearestTarget != null)
            {
                SetTarget(character.cameraTargetLock.nearestTarget);
                isLockedOn = true;
            }
        }
    }

    private void SetTarget(Enemy nearestTarget)
    {
        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;

            //Debug.Log("locked onto : " + nearestTarget.name);
        }
        else
        {
            currentTarget = null;
        }
    }

    private void ResetTargetLock()
    {

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

        character.animator.SetBool("isCombat", false);
    }

    private IEnumerator SwapWeapon()
    {
        yield return new WaitForSeconds(2f);
        character.animator.SetTrigger("drawWeapon");
        character.weaponEquipment.SetWeapon(swapWeaponTo);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class CombatState : State
{
    float gravityValue;
    float playerSpeed;

    int swapWeaponTo;

    bool isGrounded;
    bool holsterWeapon;
    bool blocking;
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
        blocking = false;
        attackState = false;
        heavyAttackState = false;
        swapWeapon = false;
        lockOnTrigger = false;
        leftLockOnTrigger = false;
        rightLockOnTrigger = false;

        input = Vector2.zero;
        targetDirection = Vector3.zero;
        lockedTargetDirection = Vector3.zero;
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

        if (blockAction.triggered)
        {
            blocking = true;
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
        if (character.isLockedOn)
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

        if (character.isLockedOn)
        {
            SetAnimationParameters(horizontalInput, verticalInput);
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

        if (blocking)
        {
            blocking = false;
            stateMachine.ChangeState(character.blockState);
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

        if (character.isLockedOn)
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
        if (character.currentLockedOnTarget == null)
            return;

        if (character.currentLockedOnTarget.isDead)
        {
            ResetLockOn();

            if (lockOnCoroutine != null)
                character.StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = character.StartCoroutine(PlayerCamera.instance.WaitFindNewTarget());
        }
    }

    private void HandleTargetLockOn()
    {
        if (lockOnTrigger && character.isLockedOn)
        {
            lockOnTrigger = false;

            PlayerCamera.instance.ClearLockOnTargets();
            SetTarget(null);
            character.isLockedOn = false;

            return;
        }

        if (lockOnTrigger && !character.isLockedOn)
        {
            lockOnTrigger = false;

            PlayerCamera.instance.FindLockOnTarget();

            if (PlayerCamera.instance.nearestTarget != null)
            {
                SetTarget(PlayerCamera.instance.nearestTarget);
                character.isLockedOn = true;
            }
        }
    }

    private void HandleTargetLockOnSwitch()
    {
        if (leftLockOnTrigger)
        {
            leftLockOnTrigger = false;

            if (character.isLockedOn)
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

            if (character.isLockedOn)
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
        character.isLockedOn = false;
        SetTarget(null);
    }

    public void SetTarget(Enemy nearestTarget)
    {
        if (nearestTarget != null)
        {
            character.currentLockedOnTarget = nearestTarget;
        }
        else
        {
            character.currentLockedOnTarget = null;
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

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        if (character.isLockedOn)
        {
            character.transform.rotation = Quaternion.LookRotation(lockedTargetDirection);
        }
        else
        {
            character.transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
}

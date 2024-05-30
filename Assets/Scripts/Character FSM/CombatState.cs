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

        moveVelocity = player.playerVelocity;
        playerSpeed = player.combatSpeed;
        isGrounded = character.controller.isGrounded;
        gravityValue = character.GRAVITY_VALUE;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.dodgeAction.triggered)
        {
            stateMachine.ChangeState(player.dodgeState);
        }

        if (player.drawWeaponAction.triggered)
        {
            holsterWeapon = true;
        }

        if (player.blockAction.triggered)
        {
            blocking = true;
        }

        if (player.heavyAttackWeaponAction.triggered)
        {
            heavyAttackState = true;
        }

        if (player.liteAttackWeaponAction.triggered && !heavyAttackState)
        {
            if (character.animator.GetBool("swapWeapon"))
                return;

            attackState = true;
        }

        if (player.weapon1Action.triggered)
        {
            swapWeaponTo = 1;
            swapWeapon = true;
        }
        if (player.weapon2Action.triggered)
        {
            swapWeaponTo = 2;
            swapWeapon = true;
        }
        if (player.weapon3Action.triggered)
        {
            swapWeaponTo = 3;
            swapWeapon = true;
        }
        if (player.weapon4Action.triggered)
        {
            swapWeaponTo = 4;
            swapWeapon = true;
        }

        if (player.lockOnAction.triggered)
        {
            lockOnTrigger = true;
        }

        // target lock on swap
        if (player.isLockedOn)
        {
            if (player.leftLockOnAction.triggered || player.rightLockOnAction.triggered)
            {
                if (player.playerInput.devices[0] is Gamepad)
                {
                    if (player.leftLockOnAction.triggered)
                    {
                        leftLockOnTrigger = true;
                    }

                    if (player.rightLockOnAction.triggered)
                    {
                        rightLockOnTrigger = true;
                    }
                }
                else if (player.playerInput.devices[0] is Keyboard)
                {
                    if (player.leftLockOnAction.ReadValue<float>() <= -PlayerCamera.Instance.mouseLockOnSwitchTreshold)
                    {
                        leftLockOnTrigger = true;
                    }

                    if (player.rightLockOnAction.ReadValue<float>() >= PlayerCamera.Instance.mouseLockOnSwitchTreshold)
                    {
                        rightLockOnTrigger = true;
                    }
                }
            }
        }

        input = player.moveAction.ReadValue<Vector2>();
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

        moveVelocity = PlayerCamera.Instance.transform.forward * verticalInput;
        moveVelocity += PlayerCamera.Instance.transform.right * horizontalInput;
        moveVelocity.Normalize();
        moveVelocity.y = 0;

        if (player.isLockedOn)
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
            stateMachine.ChangeState(player.idleState);
        }

        if (blocking)
        {
            blocking = false;
            stateMachine.ChangeState(player.blockState);
        }

        if (heavyAttackState)
        {
            heavyAttackState = false;
            //character.animator.SetTrigger("heavyAttack");
            stateMachine.ChangeState(player.heavyAttackState);
        }

        if (attackState)
        {
            attackState = false;
            //character.animator.SetTrigger("attack");
            stateMachine.ChangeState(player.liteAttackState);
        }

        if (swapWeapon)
        {
            swapWeapon = false;
            HandleWeaponSwap();
        }

        if (player.isLockedOn)
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
            stateMachine.ChangeState(player.idleState);

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
        if (player.currentLockedOnTarget == null)
            return;

        if (player.currentLockedOnTarget.isDead)
        {
            ResetLockOn();

            if (lockOnCoroutine != null)
                character.StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = character.StartCoroutine(PlayerCamera.Instance.WaitFindNewTarget());
        }
    }

    private void HandleTargetLockOn()
    {
        if (lockOnTrigger && player.isLockedOn)
        {
            lockOnTrigger = false;

            PlayerCamera.Instance.ClearLockOnTargets();
            SetTarget(null);
            player.isLockedOn = false;

            return;
        }

        if (lockOnTrigger && !player.isLockedOn)
        {
            lockOnTrigger = false;

            PlayerCamera.Instance.FindLockOnTarget();

            if (PlayerCamera.Instance.nearestTarget != null)
            {
                SetTarget(PlayerCamera.Instance.nearestTarget);
                player.isLockedOn = true;
            }
        }
    }

    private void HandleTargetLockOnSwitch()
    {
        if (leftLockOnTrigger)
        {
            leftLockOnTrigger = false;

            if (player.isLockedOn)
            {
                PlayerCamera.Instance.FindLockOnTarget();

                if (PlayerCamera.Instance.leftLockOnTarget != null)
                {
                    SetTarget(PlayerCamera.Instance.leftLockOnTarget);
                }
            }
        }

        if (rightLockOnTrigger)
        {
            rightLockOnTrigger = false;

            if (player.isLockedOn)
            {
                PlayerCamera.Instance.FindLockOnTarget();

                if (PlayerCamera.Instance.rightLockOnTarget != null)
                {
                    SetTarget(PlayerCamera.Instance.rightLockOnTarget);
                }
            }
        }
    }

    private void ResetLockOn()
    {
        player.isLockedOn = false;
        SetTarget(null);
    }

    public override void SetTarget(Enemy nearestTarget)
    {
        if (nearestTarget != null)
        {
            player.currentLockedOnTarget = nearestTarget;
        }
        else
        {
            player.currentLockedOnTarget = null;
        }

        PlayerCamera.Instance.SetLockOnCameraHeight();
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
            character.controller.Move(player.walkingSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
        }

        HandleRotation();
    }

    public override void SwapWeapon()
    {
        if (character.weaponEquipment.SetWeapon(swapWeaponTo))
        {
            player.idleState.previousWeaponSlot = swapWeaponTo;
            character.animator.SetTrigger("drawWeapon");
        }
    }

    private void SlowDownTime()
    {
        float slowTime = player.weaponSwapSlowTime;

        Time.timeScale = slowTime;
        Time.fixedDeltaTime = slowTime * Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);

        if (player.isLockedOn)
        {
            player.transform.rotation = Quaternion.LookRotation(lockedTargetDirection);
        }
        else
        {
            player.transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
}

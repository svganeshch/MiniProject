using UnityEngine;
using UnityEngine.InputSystem;

public class CombatState : State
{
    int swapWeaponTo;

    bool sprint;
    bool jump;
    bool dodge;
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

        player.walkEnabled = false;
        ResetFlags();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        HandleActionInputs();
        HandleWeaponSwapInputs();
        HandleLockOnInputs();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        PerformActions();
    }

    private void ResetFlags()
    {
        jump = dodge = holsterWeapon = blocking = attackState = heavyAttackState = swapWeapon = lockOnTrigger = leftLockOnTrigger = rightLockOnTrigger = false;
    }

    private void HandleActionInputs()
    {
        if (player.sprintAction.WasPressedThisFrame()) sprint = true;
        if (player.jumpAction.WasPressedThisFrame()) jump = true;
        if (player.dodgeAction.WasPressedThisFrame()) dodge = true;
        if (player.drawWeaponAction.WasPressedThisFrame()) holsterWeapon = true;
        if (player.blockAction.WasPressedThisFrame()) blocking = true;
        if (player.heavyAttackWeaponAction.WasPressedThisFrame()) heavyAttackState = true;

        if (player.liteAttackWeaponAction.WasPressedThisFrame() && !heavyAttackState && !character.animator.GetBool("swapWeapon"))
        {
            attackState = true;
        }
    }

    private void HandleWeaponSwapInputs()
    {
        if (player.weapon1Action.WasPressedThisFrame()) SetSwapWeapon(1);
        if (player.weapon2Action.WasPressedThisFrame()) SetSwapWeapon(2);
        if (player.weapon3Action.WasPressedThisFrame()) SetSwapWeapon(3);
        if (player.weapon4Action.WasPressedThisFrame()) SetSwapWeapon(4);
    }

    private void SetSwapWeapon(int weaponSlot)
    {
        swapWeaponTo = weaponSlot;
        swapWeapon = true;
    }

    private void HandleLockOnInputs()
    {
        if (player.lockOnAction.WasPressedThisFrame()) lockOnTrigger = true;

        if (player.isLockedOn)
        {
            if (player.leftLockOnAction.WasPressedThisFrame() || player.rightLockOnAction.WasPressedThisFrame())
            {
                if (player.playerInput.devices[0] is Gamepad)
                {
                    leftLockOnTrigger = player.leftLockOnAction.WasPressedThisFrame();
                    rightLockOnTrigger = player.rightLockOnAction.WasPressedThisFrame();
                }
                else if (player.playerInput.devices[0] is Keyboard)
                {
                    leftLockOnTrigger = player.leftLockOnAction.ReadValue<float>() <= -PlayerCamera.Instance.mouseLockOnSwitchTreshold;
                    rightLockOnTrigger = player.rightLockOnAction.ReadValue<float>() >= PlayerCamera.Instance.mouseLockOnSwitchTreshold;
                }
            }
        }
    }

    private void PerformActions()
    {
        if (sprint) ChangeStateAndResetFlag(ref sprint, player.sprintState);
        if (jump) ChangeStateAndResetFlag(ref jump, player.jumpState);
        if (dodge) ChangeStateAndResetFlag(ref dodge, player.dodgeState);
        if (holsterWeapon) HandleHolsterWeapon();
        if (blocking) ChangeStateAndResetFlag(ref blocking, player.blockState);
        if (heavyAttackState) ChangeStateAndResetFlag(ref heavyAttackState, player.heavyAttackState);
        if (attackState) ChangeStateAndResetFlag(ref attackState, player.liteAttackState);
        if (swapWeapon) HandleWeaponSwap();
        if (player.isLockedOn) CheckLockOn();
        if (lockOnTrigger) HandleTargetLockOn();
        if (leftLockOnTrigger || rightLockOnTrigger) HandleTargetLockOnSwitch();
    }

    private void ChangeStateAndResetFlag(ref bool flag, State newState)
    {
        flag = false;
        stateMachine.ChangeState(newState);
    }

    private void HandleHolsterWeapon()
    {
        holsterWeapon = false;
        ResetLockOn();
        player.playerAnimatorManager.PlayWeaponHolsterAction();
        stateMachine.ChangeState(player.idleState);
    }

    private void HandleWeaponSwap()
    {
        swapWeapon = false;

        if (character.weaponEquipment.GetCurrentWeapon().weaponSlot == swapWeaponTo)
        {
            HandleHolsterWeapon();
            return;
        }

        if (character.weaponEquipment.GetWeaponWithSlot(swapWeaponTo).Enabled)
        {
            SlowDownTime();
            character.characterAnimatorManager.HolsterSpeed = character.weaponEquipment.weaponSwapSpeed;
            character.characterAnimatorManager.DrawSpeed = character.weaponEquipment.weaponSwapSpeed;

            character.characterAnimatorManager.SwapWeapon = true;
            player.playerAnimatorManager.PlayWeaponHolsterAction();
        }
    }

    private void CheckLockOn()
    {
        if (player.currentLockedOnTarget == null || !player.currentLockedOnTarget.isDead) return;

        ResetLockOn();

        if (lockOnCoroutine != null)
            character.StopCoroutine(lockOnCoroutine);

        lockOnCoroutine = character.StartCoroutine(PlayerCamera.Instance.WaitFindNewTarget());

        Debug.Log("checking lock on");
    }

    private void HandleTargetLockOn()
    {
        lockOnTrigger = false;

        if (player.isLockedOn)
        {
            PlayerCamera.Instance.ClearLockOnTargets();
            SetTarget(null);
            player.isLockedOn = false;
        }
        else
        {
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
                SetTarget(PlayerCamera.Instance.leftLockOnTarget);
            }
        }

        if (rightLockOnTrigger)
        {
            rightLockOnTrigger = false;
            if (player.isLockedOn)
            {
                PlayerCamera.Instance.FindLockOnTarget();
                SetTarget(PlayerCamera.Instance.rightLockOnTarget);
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
        player.currentLockedOnTarget = nearestTarget;
        PlayerCamera.Instance.SetLockOnCameraHeight();

        character.hudManager.SetLockedOnTargetCrosshair(nearestTarget);
    }

    public override void SwapWeapon()
    {
        if (character.weaponEquipment.SetWeapon(swapWeaponTo))
        {
            player.idleState.previousWeaponSlot = swapWeaponTo;
            player.playerAnimatorManager.PlayWeaponDrawAction();

            isSwappingWeapon = true;
        }
    }

    private void SlowDownTime()
    {
        float slowTime = player.weaponSwapSlowTime;
        Time.timeScale = slowTime;
        Time.fixedDeltaTime = slowTime * Time.deltaTime;
    }
}
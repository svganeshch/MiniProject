using UnityEngine;
using UnityEngine.InputSystem;

public class CombatState : State
{
    int swapWeaponTo;

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

        jump = false;
        dodge = false;
        holsterWeapon = false;
        blocking = false;
        attackState = false;
        heavyAttackState = false;
        swapWeapon = false;
        lockOnTrigger = false;
        leftLockOnTrigger = false;
        rightLockOnTrigger = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.jumpAction.triggered)
        {
            jump = true;
        }

        if (player.dodgeAction.triggered)
        {
            dodge = true;
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
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (jump)
        {
            jump = false;
            stateMachine.ChangeState(player.jumpState);
        }

        if (dodge)
        {
            dodge = false;
            stateMachine.ChangeState(player.dodgeState);
        }

        if (holsterWeapon)
        {
            holsterWeapon = false;

            ResetLockOn();
            player.playerAnimatorManager.PlayWeaponHolsterAction();
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

            player.playerAnimatorManager.PlayWeaponHolsterAction();
            stateMachine.ChangeState(player.idleState);

            return;
        }

        if (character.weaponEquipment.GetWeaponWithSlot(swapWeaponTo).Enabled)
        {
            SlowDownTime();

            character.animator.SetFloat("holsterSpeed", character.weaponEquipment.weaponSwapSpeed);
            character.animator.SetFloat("drawSpeed", character.weaponEquipment.weaponSwapSpeed);
            character.animator.SetBool("swapWeapon", true);

            player.playerAnimatorManager.PlayWeaponHolsterAction();
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

    public override void SwapWeapon()
    {
        if (character.weaponEquipment.SetWeapon(swapWeaponTo))
        {
            player.idleState.previousWeaponSlot = swapWeaponTo;
            player.playerAnimatorManager.PlayWeaponDrawAction();
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
    }
}

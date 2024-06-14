public class IdleState : State
{
    bool dodge;
    bool jump;
    bool sprint;
    bool drawWeapon;

    int weaponSlot;

    public IdleState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dodge = false;
        jump = false;
        sprint = false;
        drawWeapon = false;

        if (previousWeaponSlot != 0)
        {
            weaponSlot = previousWeaponSlot;
        }
        else
        {
            weaponSlot = defaultWeaponSlot;
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.jumpAction.WasPressedThisFrame())
            jump = true;

        if (player.sprintAction.WasPressedThisFrame())
            sprint = true;

        if (player.dodgeAction.WasPressedThisFrame())
        {
            dodge = true;
        }

        if (player.drawWeaponAction.WasPressedThisFrame())
            drawWeapon = true;

        if (player.weapon1Action.WasPressedThisFrame())
        {
            weaponSlot = 1;
            drawWeapon = true;
        }
        if (player.weapon2Action.WasPressedThisFrame())
        {
            weaponSlot = 2;
            drawWeapon = true;
        }
        if (player.weapon3Action.WasPressedThisFrame())
        {
            weaponSlot = 3;
            drawWeapon = true;
        }
        if (player.weapon4Action.WasPressedThisFrame())
        {
            weaponSlot = 4;
            drawWeapon = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (dodge)
        {
            dodge = false;
            stateMachine.ChangeState(player.dodgeState);
        }

        if (jump)
        {
            jump = false;

            if (player.playerAnimatorManager.IsGrounded)
                stateMachine.ChangeState(player.jumpState);
        }

        if (sprint)
        {
            sprint = false;
            stateMachine.ChangeState(player.sprintState);
        }

        if (drawWeapon)
        {
            drawWeapon = false;
            if (character.weaponEquipment.SetWeapon(weaponSlot))
            {
                previousWeaponSlot = weaponSlot;

                player.playerAnimatorManager.PlayWeaponDrawAction();
                stateMachine.ChangeState(player.combatState);
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
    }

    public override void Exit()
    {
        base.Exit();
    }
}

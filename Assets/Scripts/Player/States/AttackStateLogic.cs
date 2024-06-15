using UnityEngine;

public class AttackStateLogic : State
{
    protected bool liteAttack;

    bool dodge;
    bool block;

    bool input_que_active = false;
    float default_que_input_timer = 0.35f;
    float que_input_timer;

    bool attack_que = false;

    public AttackStateLogic(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dodge = false;
        block = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.dodgeAction.WasPressedThisFrame())
        {
            dodge = true;
        }

        if (player.blockAction.WasPressedThisFrame())
        {
            block = true;
        }

        if (player.liteAttackWeaponAction.WasPressedThisFrame())
        {
            if (!attack_que)
            {
                QueInput(ref attack_que);
                return;
            }
            liteAttack = true;
        }

        HandleQuedInputs();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.canPerformAction)
        {
            HandleRotation();

            if (dodge)
            {
                player.canPerformAction = false;
                dodge = false;
                stateMachine.ChangeState(player.dodgeState);
            }

            if (block)
            {
                player.canPerformAction = false;
                block = false;
                stateMachine.ChangeState(player.blockState);
            }
        }

        if (liteAttack)
        {
            liteAttack = false;

            if (player.canCombo)
            {
                player.canCombo = false;
                player.playerAnimatorManager.PlayLiteAttackAction(true);
            }
            else
            {
                if (!player.isAttacking)
                {
                    player.playerAnimatorManager.PlayLiteAttackAction(false);
                }
            }
        }
    }

    private void QueInput(ref bool quedInput)
    {
        ResetQueFlags();

        quedInput = true;
        que_input_timer = default_que_input_timer;
        input_que_active = true;
    }

    private void ProcessQuedInputs()
    {
        if (attack_que) liteAttack = true;
    }

    private void HandleQuedInputs()
    {
        if (input_que_active)
        {
            if (que_input_timer > 0)
            {
                que_input_timer -= Time.deltaTime;
                ProcessQuedInputs();
            }
            else
            {
                ResetQueFlags();
            }
        }
    }

    private void ResetQueFlags()
    {
        attack_que = false;

        input_que_active = false;
        que_input_timer = 0;
    }

    public override void Exit()
    {
        base.Exit();

        liteAttack = false;

        ResetQueFlags();
    }
}

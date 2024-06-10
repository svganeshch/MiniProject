public class AttackStateLogic : State
{
    protected bool canComboAttack;

    bool attack;
    bool dodge;
    bool block;

    private float clipTime;
    private float attackCancelTreshold;
    private float attackComboTreshold;

    int animatorActionsLayerindex;

    public AttackStateLogic(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        attack = false;
        dodge = false;
        block = false;

        clipTime = 0f;

        attackCancelTreshold = player.attackCancelTreshold;
        attackComboTreshold = player.attackComboTreshold;

        player.characterSfxManager.PlayWeaponSlashSound();

        animatorActionsLayerindex = player.animator.GetLayerIndex("Action Override");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.dodgeAction.triggered)
        {
            dodge = true;
        }

        if (player.blockAction.triggered)
        {
            block = true;
        }

        if (player.liteAttackWeaponAction.triggered)
        {
            attack = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.animator.GetCurrentAnimatorClipInfo(animatorActionsLayerindex).Length == 0) return;
        clipTime = player.animator.GetCurrentAnimatorStateInfo(animatorActionsLayerindex).normalizedTime;

        //Debug.Log("clip time : " + clipTime);

        if (clipTime <= attackCancelTreshold * 0.9f)
        {
            HandleRotation();

            if (dodge)
            {
                dodge = false;
                stateMachine.ChangeState(player.dodgeState);
            }

            if (block)
            {
                block = false;
                stateMachine.ChangeState(player.blockState);
            }
        }

        if (clipTime >= attackComboTreshold * 0.9f)
        {
            if (attack)
            {
                attack = false;
                canComboAttack = true;
                stateMachine.ChangeState(this, true);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

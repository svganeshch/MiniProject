public class HitState : State
{
    private bool dodge = false;

    public HitState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dodge = false;

        player.playerAnimatorManager.PlayHitAction();
        player.characterSfxManager.PlayWeaponHitFleshSound();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.dodgeAction.WasPressedThisFrame())
        {
            dodge = true;
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
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class DodgeState : State
{
    bool block;

    public DodgeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        block = false;

        if (player.playerAnimatorManager.CombatBool)
        {
            player.playerAnimatorManager.PlayCombatDodgeAction();
        }
        else
        {
            player.playerAnimatorManager.PlayDodgeAction();
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.blockAction.triggered)
        {
            block = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (block)
        {
            block = false;
            stateMachine.ChangeState(player.blockState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

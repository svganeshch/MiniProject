public class BlockState : State
{
    public BlockState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.playerAnimatorManager.PlayBlockAction();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.blockAction.WasReleasedThisFrame())
        {
            player.animator.SetTrigger("releaseBlock");
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
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

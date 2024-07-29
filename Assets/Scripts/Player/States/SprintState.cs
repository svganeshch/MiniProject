public class SprintState : State
{
    bool sprint;
    bool sprintJump;

    public SprintState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();

        sprint = true;
        sprintJump = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.sprintAction.WasPressedThisFrame() || player.inputValues.sqrMagnitude == 0f)
        {
            sprint = false;
        }

        if (player.jumpAction.WasPressedThisFrame())
        {
            sprintJump = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!sprint)
        {
            if (player.playerAnimatorManager.CombatBool)
            {
                stateMachine.ChangeState(player.combatState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
        }

        if (sprintJump)
        {
            stateMachine.ChangeState(player.jumpState);
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

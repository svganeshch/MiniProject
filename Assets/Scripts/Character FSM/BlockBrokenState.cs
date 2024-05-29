public class BlockBrokenState : State
{
    public bool blockBrokenDone = false;
    public BlockBrokenState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        blockBrokenDone = false;

        character.animator.applyRootMotion = true;
        character.animator.SetTrigger("block_broken");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (blockBrokenDone)
        {
            stateMachine.ChangeState(character.combatState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}

public class EnemyHitState : State
{
    public EnemyHitState(Enemy _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        character.animator.applyRootMotion = true;
        character.animator.SetTrigger("damage");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (hitDone)
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

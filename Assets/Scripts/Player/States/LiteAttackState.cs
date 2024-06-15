public class LiteAttackState : AttackStateLogic
{
    public LiteAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        liteAttack = true;
    }
}

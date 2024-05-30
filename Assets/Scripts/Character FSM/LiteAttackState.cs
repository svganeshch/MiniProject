public class LiteAttackState : AttackStateLogic
{
    public LiteAttackState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();

        character.animator.SetTrigger("liteAttack");
    }
}

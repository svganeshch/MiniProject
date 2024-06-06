public class EnemyHitState : State
{
    public EnemyHitState(Enemy _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.enemyAnimatorManager.PlayHitAction();
        enemy.enemySfxManager.PlayWeaponHitFleshSound();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

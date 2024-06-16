public class EnemySprintState : State
{
    public EnemySprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.recallTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance + 0.5f)
        {
            stateMachine.ChangeState(enemy.combatState);
        }

        enemy.HandleRecallDistanceChecks();
    }
}
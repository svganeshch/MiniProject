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

        enemy.navMeshAgent.destination = enemy.currentTarget.transform.position;

        if (enemy.navMeshAgent.remainingDistance <= enemy.sprintDistance / 3)
        {
            stateMachine.ChangeState(enemy.combatState);
        }

        enemy.CheckRecallDistance();
    }
}
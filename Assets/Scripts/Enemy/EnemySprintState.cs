using UnityEngine;

public class EnemySprintState : State
{
    float recallTimer = 0f;

    public EnemySprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        recallTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        enemy.navMeshAgent.destination = enemy.currentTarget.transform.position;

        if (enemy.navMeshAgent.remainingDistance <= enemy.sprintDistance / 3)
        {
            stateMachine.ChangeState(enemy.combatState);
        }

        CheckRecallDistance();
    }

    private void CheckRecallDistance()
    {
        recallTimer += Time.deltaTime;

        if (recallTimer >= 5)
        {
            if (enemy.navMeshAgent.remainingDistance >= enemy.recallDistance)
            {
                enemy.currentTarget = null;
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }
}
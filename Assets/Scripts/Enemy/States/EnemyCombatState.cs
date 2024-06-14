using UnityEngine;

public class EnemyCombatState : State
{
    public EnemyCombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
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

        ResetEnemy();

        enemy.RecallDistanceChecks();

        CheckAttackDistance();
    }

    private void ResetEnemy()
    {
        if (enemy.isDead)
        {
            enemy.navMeshAgent.enabled = false;
            enemy.currentTarget = null;
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        else if (enemy.currentTarget == null)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
    }

    private void CheckAttackDistance()
    {
        float remainingDistance = enemy.navMeshAgent.remainingDistance;

        if (remainingDistance <= enemy.navMeshAgent.stoppingDistance + 0.2f)
        {
            if (enemy.currentTarget.inputValues == Vector2.zero)
            {
                stateMachine.ChangeState(enemy.strafeState);
            }
        }
        else if (remainingDistance >= enemy.sprintDistance)
        {
            stateMachine.ChangeState(enemy.sprintState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
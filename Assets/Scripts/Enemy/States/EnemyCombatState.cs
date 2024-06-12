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

        enemy.navMeshAgent.destination = enemy.currentTarget.transform.position;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!enemy.currentTarget) stateMachine.ChangeState(enemy.idleState);

        SetNavAgent();
        CheckAttackDistance();

        if (!enemy.isLockedOn)
            enemy.CheckRecallDistance();
    }

    private void SetNavAgent()
    {
        if (enemy.isDead)
        {
            enemy.navMeshAgent.enabled = false;
            enemy.currentTarget = null;
            stateMachine.ChangeState(enemy.idleState);

            return;
        }

        // new nav logic to control with character controller
        enemy.navMeshAgent.destination = enemy.currentTarget.transform.position;
    }

    private void CheckAttackDistance()
    {
        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance + 0.2f)
        {
            if (enemy.currentTarget.inputValues == Vector2.zero)
                stateMachine.ChangeState(enemy.strafeState);
        }
        else if (enemy.navMeshAgent.remainingDistance >= enemy.sprintDistance)
        {
            stateMachine.ChangeState(enemy.sprintState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

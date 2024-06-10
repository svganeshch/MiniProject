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

        SetNavAgent();
        CheckAttackDistance();
        enemy.CheckRecallDistance();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
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

        //if (enemy.idleState.currentTarget != null )
        //{
        //    enemy.idleState.targetDirection = enemy.idleState.currentTarget.transform.position - enemy.transform.position;
        //    enemy.idleState.viewableAngle = GetAngleOfTarget(enemy.transform, enemy.idleState.targetDirection);
        //}

        //Debug.Log("enemy controller : " + character.controller.velocity);
        //Debug.Log("chasing player");


    }

    private void CheckAttackDistance()
    {
        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        else if (enemy.navMeshAgent.remainingDistance >= enemy.sprintDistance)
        {
            stateMachine.ChangeState(enemy.sprintState);
        }
    }

    public float GetAngleOfTarget(Transform transform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle = Vector3.Angle(transform.forward, targetDirection);
        Vector3 cross = Vector3.Cross(transform.forward, targetDirection);

        if (cross.y < 0)
            viewableAngle = -viewableAngle;

        return viewableAngle;
    }

    public override void Exit()
    {
        base.Exit();
    }
}

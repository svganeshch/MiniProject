using UnityEngine;

public class EnemyCombatState : State
{
    Vector3 desiredVelocity;
    Vector3 lookDirection;

    Quaternion lookRotation;

    public EnemyCombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.navMeshAgent.destination = enemy.currentTarget.transform.position;
        enemy.navMeshAgent.updatePosition = false;
        enemy.navMeshAgent.updateRotation = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        SetNavAgent();
        CheckAttackDistance();
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
        desiredVelocity = enemy.navMeshAgent.desiredVelocity;

        lookDirection = enemy.currentTarget.transform.position - enemy.transform.position;
        lookDirection.y = 0;

        lookRotation = Quaternion.LookRotation(lookDirection);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.rotationDampTime);

        enemy.controller.Move(enemy.runningSpeed * Time.deltaTime * desiredVelocity.normalized);
        enemy.navMeshAgent.velocity = enemy.controller.velocity;

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
        float remainingDistance = Vector3.Distance(enemy.transform.position, enemy.navMeshAgent.destination);

        if (remainingDistance <= enemy.navMeshAgent.stoppingDistance)
        {
            stateMachine.ChangeState(enemy.attackState);
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

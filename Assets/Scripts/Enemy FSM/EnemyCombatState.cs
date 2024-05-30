using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatState : State
{
    public EnemyCombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        character.animator.applyRootMotion = true;

        Debug.Log("Entered combat state");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.currentTarget == null)
        {
            stateMachine.ChangeState(character.idleState);
        }

        CheckAttackDistance();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        SetNavAgent();
    }

    private void SetNavAgent()
    {
        //if (enemy.idleState.viewableAngle < enemy.idleState.minimumFOV
        //    || enemy.idleState.viewableAngle > enemy.idleState.maximumFOV)
        //{
        //    enemy.idleState.PivotTowardsTarget();
        //}
        enemy.transform.rotation = enemy.navMesh.transform.rotation;

        //enemy.navMesh.SetDestination(enemy.idleState.currentTarget.transform.position);
        NavMeshPath navMeshPath = new NavMeshPath();
        enemy.navMesh.CalculatePath(enemy.currentTarget.transform.position, navMeshPath);
        enemy.navMesh.SetPath(navMeshPath);

        enemy.navMesh.transform.localPosition = Vector3.zero;
        enemy.navMesh.transform.localRotation = Quaternion.identity;

        //if (enemy.idleState.currentTarget != null )
        //{
        //    enemy.idleState.targetDirection = enemy.idleState.currentTarget.transform.position - enemy.transform.position;
        //    enemy.idleState.viewableAngle = GetAngleOfTarget(enemy.transform, enemy.idleState.targetDirection);
        //}

        //Debug.Log("enemy controller : " + character.controller.velocity);
        moveAmount = Mathf.Clamp01(Mathf.Abs(character.controller.velocity.z) + Mathf.Abs(character.controller.velocity.x));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        SetAnimationParameters(0, moveAmount);
        //Debug.Log("chasing player");

        if (enemy.isDead)
        {
            enemy.navMesh.enabled = false;
            SetAnimationParameters(0, 0);
            enemy.currentTarget = null;
            stateMachine.ChangeState(character.idleState);
        }
    }

    private void CheckAttackDistance()
    {
        float remainingDistance = Vector3.Distance(enemy.transform.position, enemy.navMesh.destination);

        if (remainingDistance <= enemy.navMesh.stoppingDistance)
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

        character.animator.applyRootMotion = false;
    }
}

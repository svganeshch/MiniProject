using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPursueState : EnemyState
{
    public EnemyPursueState(Enemy _enemy, EnemyStateMachine _enemyStateMachine) : base(_enemy, _enemyStateMachine)
    {
        enemy = _enemy;
        enemyStateMachine = _enemyStateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.animator.applyRootMotion = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.idleState.currentTarget == null)
        {
            enemyStateMachine.ChangeState(enemy.idleState);
        }

        SetNavAgent();
        CheckAttackDistance();
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
        enemy.navMesh.CalculatePath(enemy.idleState.currentTarget.transform.position, navMeshPath);
        enemy.navMesh.SetPath(navMeshPath);

        enemy.navMesh.transform.localPosition = Vector3.zero;
        enemy.navMesh.transform.localRotation = Quaternion.identity;

        //if (enemy.idleState.currentTarget != null )
        //{
        //    enemy.idleState.targetDirection = enemy.idleState.currentTarget.transform.position - enemy.transform.position;
        //    enemy.idleState.viewableAngle = GetAngleOfTarget(enemy.transform, enemy.idleState.targetDirection);
        //}

        enemy.animator.SetFloat("speed", 0.5f);
        //Debug.Log("chasing player");

        if (enemy.isDead)
        {
            enemy.navMesh.enabled = false;
            enemy.animator.SetFloat("speed", 0);
            enemy.idleState.currentTarget = null;
            enemyStateMachine.ChangeState(enemy.idleState);
        }
    }

    private void CheckAttackDistance()
    {
        float remainingDistance = Vector3.Distance(enemy.transform.position, enemy.navMesh.destination);

        if (remainingDistance <= enemy.navMesh.stoppingDistance)
        {
            enemyStateMachine.ChangeState(enemy.attackState);
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

        enemy.animator.applyRootMotion = false;
    }
}

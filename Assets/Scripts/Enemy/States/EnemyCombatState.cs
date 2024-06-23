using UnityEngine;

public class EnemyCombatState : State
{
    Vector3 startPos;

    public EnemyCombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.recallTimer = 0f;
        startPos = enemy.transform.position;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        ResetEnemy();

        if (enemy.isInCoolDown)
        {
            enemy.enemyMovementManager.manualUpdate = true;
            FallBack();

            return;
        }

        HandleTargetDistanceChecks();
        enemy.HandleRecallDistanceChecks();
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

    private void FallBack()
    {
        if (enemy.navMeshAgent.remainingDistance < enemy.attackRange)
        {
            Vector3 fallbackDirection = enemy.currentTarget.transform.position - enemy.transform.position;
            fallbackDirection.y = 0f;
            fallbackDirection.Normalize();

            enemy.enemyMovementManager.UpdateMovement(enemy.walkingSpeed, -fallbackDirection, 2);
        }
        else
        {
            enemy.isInCoolDown = false;
            enemy.enemyMovementManager.manualUpdate = false;
        }
    }

    private void HandleTargetDistanceChecks()
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
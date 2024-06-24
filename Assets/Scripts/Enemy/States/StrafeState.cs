using UnityEngine;

public class StrafeState : State
{
    float strafeAttackTimer = 0f;

    Vector3 targetPreviousPosition;
    Vector3 rightAngle;
    Vector3 leftAngle;
    Vector3 targetAngle;
 
    float remainingDistanceToTheAnglePoint = 0;

    public StrafeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.enemyMovementManager.manualUpdate = true;
        enemy.isLockedOn = true;
 
        strafeAttackTimer = 0f;

        targetPreviousPosition = enemy.currentTarget.transform.position;
        SetStrafeAngle();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckDistance();
        HandleStrafe();

        enemy.navMeshAgent.nextPosition = enemy.transform.position;
    }

    private void HandleStrafe()
    {
        strafeAttackTimer += Time.deltaTime;

        if (enemy.currentTarget.transform.position != targetPreviousPosition)
        {
            SetStrafeAngle();
            targetPreviousPosition = enemy.currentTarget.transform.position;
        }

        remainingDistanceToTheAnglePoint = Vector3.Distance(enemy.transform.position, targetAngle);

        if (remainingDistanceToTheAnglePoint <= 0.25f)
        {
            // Switch to the other angle
            if (targetAngle == rightAngle)
            {
                targetAngle = leftAngle;
            }
            else
            {
                targetAngle = rightAngle;
            }
        }

        Vector3 strafeDirectionVector = targetAngle - enemy.transform.position;
        strafeDirectionVector.y = 0f;
        strafeDirectionVector.Normalize();

        enemy.enemyMovementManager.UpdateMovement(enemy.strafeSpeed, strafeDirectionVector, remainingDistanceToTheAnglePoint);

        if (strafeAttackTimer > enemy.strafeAttackDuration)
        {
            if (Random.value <= enemy.attackProbability)
                stateMachine.ChangeState(enemy.attackState);

            strafeAttackTimer = 0;
        }
    }

    private void SetStrafeAngle()
    {
        Transform center = enemy.currentTarget.transform;
        float radius = Vector3.Distance(enemy.transform.position, enemy.currentTarget.transform.position);

        rightAngle = center.position + Quaternion.AngleAxis(enemy.strafeAngle, Vector3.up) * center.forward * radius;
        leftAngle = center.position + Quaternion.AngleAxis(-enemy.strafeAngle, Vector3.up) * center.forward * radius;

        float distanceToRightAngle = Vector3.Distance(enemy.transform.position, rightAngle);
        float distanceToLeftAngle = Vector3.Distance(enemy.transform.position, leftAngle);

        if (distanceToRightAngle < distanceToLeftAngle)
        {
            targetAngle = rightAngle;
        }
        else
        {
            targetAngle = leftAngle;
        }
    }

    private void CheckDistance()
    {
        if (enemy.navMeshAgent.remainingDistance > enemy.attackRange + 2f)
        {
            stateMachine.ChangeState(enemy.combatState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.enemyMovementManager.manualUpdate = false;
        enemy.isLockedOn = false;
    }

    private void DrawCircle(Vector3 center, float radius, Color color)
    {
        int segments = 100;
        float angle = 0f;
        float angleStep = 360f / segments;

        Vector3 previousPoint = center + new Vector3(Mathf.Cos(0f) * radius, 0f, Mathf.Sin(0f) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle += angleStep;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius);
            Debug.DrawLine(previousPoint, newPoint, color);
            previousPoint = newPoint;
        }
    }

    private void DrawAngleLines(Vector3 center, Vector3 rightAngle, Vector3 leftAngle, Color color)
    {
        Debug.DrawLine(center, rightAngle, color);
        Debug.DrawLine(center, leftAngle, color);

        Debug.DrawLine(rightAngle, leftAngle, color);
    }

    public override void OnDrawGizmos()
    {
        DrawCircle(enemy.currentTarget.transform.position, Vector3.Distance(enemy.transform.position, enemy.currentTarget.transform.position), Color.red);
        DrawAngleLines(enemy.currentTarget.transform.position, rightAngle, leftAngle, Color.blue);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(rightAngle, 0.25f);
        Gizmos.DrawSphere(leftAngle, 0.25f);
    }
}
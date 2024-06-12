using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    Enemy enemy;

    Vector3 desiredVelocity;
    Vector3 lookDirection;

    Quaternion lookRotation;

    float viewableAngle;
    float speed = 0;
    float speedFactor;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public override void Start()
    {
        base.Start();

        enemy.navMeshAgent.updatePosition = false;
        enemy.navMeshAgent.updateRotation = false;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.currentTarget != null)
        {
            if (viewableAngle < enemy.minimumFOV || viewableAngle > enemy.maximumFOV)
                PivotTowardsTarget(enemy.currentTarget.transform);
        }
    }

    protected override void HandleGroundedMovement()
    {
        if (!enemy.canMove)
            return;

        desiredVelocity = enemy.navMeshAgent.desiredVelocity;

        speedFactor = Mathf.Clamp01(enemy.navMeshAgent.remainingDistance / enemy.slowDownThreshold);
        speedFactor = Mathf.Max(speedFactor, enemy.minSpeedFactor);

        if (enemy.characterStateMachine.currentState == enemy.sprintState)
        {
            // sprint speed
            speed = enemy.sprintSpeed;
        }
        else if (enemy.characterStateMachine.currentState == enemy.combatState)
        {
            // combat speed
            speed = enemy.combatSpeed;
        }
        else
        {
            if (enemy.moveAmount > 0.5f)
            {
                // running speed
                speed = enemy.runningSpeed;
            }
            else if (enemy.moveAmount <= 0.5f)
            {
                // walking speed
                speed = enemy.walkingSpeed;
            }
        }
        enemy.controller.Move(speed * speedFactor * Time.deltaTime * desiredVelocity.normalized);
        enemy.navMeshAgent.velocity = enemy.controller.velocity;

        GetMovementInput();
    }

    public override void GetMovementInput()
    {
        horizontalInput = enemy.controller.velocity.x;
        verticalInput = enemy.controller.velocity.z;

        enemy.moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (enemy.characterStateMachine.currentState == enemy.idleState)
        {
            enemy.moveAmount = 0.5f;
        }

        enemy.moveAmount *= speedFactor;

        if (!enemy.isLockedOn)
        {
            if (enemy.characterStateMachine.currentState == enemy.sprintState)
            {
                enemy.moveAmount = 1.5f;
                enemy.moveAmount *= speedFactor;
            }

            enemy.enemyAnimatorManager.SetAnimatorParameters(0, enemy.moveAmount);
        }
        else
        {
            enemy.enemyAnimatorManager.SetAnimatorParameters(horizontalInput, verticalInput);
        }
    }

    protected override void HandleRotation()
    {
        if (!enemy.canRotate)
            return;

        lookDirection = enemy.navMeshAgent.destination - enemy.transform.position;
        lookDirection.y = 0;

        lookRotation = Quaternion.LookRotation(lookDirection);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.rotationDampTime);
    }

    public void PivotTowardsTarget(Transform target)
    {
        Vector3 targetDirection = target.position - enemy.transform.position;
        viewableAngle = GetAngleOfTarget(enemy.transform, targetDirection);

        //Debug.Log("va : " + viewableAngle);

        if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            // Right 90
            enemy.enemyAnimatorManager.PlayPivotAction(90);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            // Left 90
            enemy.enemyAnimatorManager.PlayPivotAction(-90);
        }
        else if (viewableAngle >= 146 && viewableAngle <= 180)
        {
            // Right 180
            enemy.enemyAnimatorManager.PlayPivotAction(180);
        }
        else if (viewableAngle <= -146 && viewableAngle >= -180)
        {
            // Left 180
            enemy.enemyAnimatorManager.PlayPivotAction(-180);
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
}

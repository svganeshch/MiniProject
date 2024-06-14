using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    private Enemy enemy;
    private Vector3 desiredVelocity;
    private Vector3 lookDirection;
    private Vector3 targetDirection;
    private Quaternion lookRotation;
    private float speed;

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
        HandleGroundedMovement();
        HandleGroundCheck();
        HandleRotation();

        //targetDirection = enemy.navMeshAgent.destination - enemy.transform.position;
        ////Debug.Log($"Viewable Angle: {viewableAngle}, isPivoting: {enemy.isPivoting}");
        //if (viewableAngle < enemy.minimumFOV || viewableAngle > enemy.maximumFOV)
        //    PivotTowardsTarget();
    }

    protected override void HandleGroundedMovement()
    {
        if (!enemy.canMove) return;

        desiredVelocity = enemy.navMeshAgent.desiredVelocity;
        float speedFactor = Mathf.Clamp01(enemy.navMeshAgent.remainingDistance / enemy.slowDownThreshold);
        speedFactor = Mathf.Max(speedFactor, enemy.minSpeedFactor);

        switch (enemy.characterStateMachine.currentState)
        {
            case var state when state == enemy.sprintState:
                speed = enemy.sprintSpeed;
                break;
            case var state when state == enemy.combatState:
                speed = enemy.combatSpeed;
                break;
            default:
                speed = enemy.moveAmount > 0.5f ? enemy.runningSpeed : enemy.walkingSpeed;
                break;
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

        enemy.moveAmount *= Mathf.Clamp01(enemy.navMeshAgent.remainingDistance / enemy.slowDownThreshold);

        if (!enemy.isLockedOn)
        {
            if (enemy.characterStateMachine.currentState == enemy.sprintState)
            {
                enemy.moveAmount = 1.5f * Mathf.Clamp01(enemy.navMeshAgent.remainingDistance / enemy.slowDownThreshold);
            }

            enemy.enemyAnimatorManager.SetAnimatorParameters(0, enemy.moveAmount, true);
        }
        else
        {
            enemy.enemyAnimatorManager.SetAnimatorParameters(horizontalInput, verticalInput);
        }
    }

    protected override void HandleRotation()
    {
        if (!enemy.canRotate) return;

        lookDirection = enemy.navMeshAgent.destination - enemy.transform.position;
        lookDirection.y = 0;

        lookRotation = Quaternion.LookRotation(lookDirection);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.rotationDampTime);
    }

    public void PivotTowardsTarget()
    {
        if (enemy.isPivoting)
        {
            Debug.Log("Already pivoting, skipping pivot.");
            return;
        }

        float angle = GetAngleOfTarget(enemy.transform, targetDirection);

        if (angle >= 61 && angle <= 110)
        {
            enemy.enemyAnimatorManager.PlayPivotAction(90);
        }
        else if (angle <= -61 && angle >= -110)
        {
            enemy.enemyAnimatorManager.PlayPivotAction(-90);
        }
        else if (angle >= 146 || angle <= -146)
        {
            enemy.enemyAnimatorManager.PlayPivotAction(angle >= 0 ? 180 : -180);
        }
    }

    public float GetAngleOfTarget(Transform transform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle = Vector3.Angle(transform.forward, targetDirection);
        Vector3 cross = Vector3.Cross(transform.forward, targetDirection);

        return cross.y < 0 ? -viewableAngle : viewableAngle;
    }
}
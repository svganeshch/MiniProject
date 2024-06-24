using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    [HideInInspector] public bool manualUpdate = false;

    private Enemy enemy;
    private Vector3 desiredVelocity;
    private Vector3 lookDirection;
    private Vector3 targetDirection;
    private Quaternion lookRotation;
    private float speed;
    private float slowDownThreshold;
    private float speedFactor;

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
        if (desiredVelocity == Vector3.zero) return;

        switch (enemy.characterStateMachine.currentState)
        {
            case var state when state == enemy.sprintState:
                speed = enemy.sprintSpeed;
                slowDownThreshold = enemy.sprintSlowDownThreshold;
                break;
            case var state when state == enemy.combatState:
                speed = enemy.combatSpeed;
                break;
            case var state when state == enemy.strafeState:
                speed = enemy.strafeSpeed;
                slowDownThreshold = enemy.strafeSlowDownThreshold;
                break;
            default:
                speed = enemy.moveAmount > 0.5f ? enemy.runningSpeed : enemy.walkingSpeed;
                slowDownThreshold = enemy.slowDownThreshold;
                break;
        }

        if (!manualUpdate)
        {
            UpdateMovement(speed, desiredVelocity.normalized, enemy.navMeshAgent.remainingDistance);
        }

        //Debug.Log(enemy.navMeshAgent.remainingDistance);
    }

    public void UpdateMovement(float speed, Vector3 direction, float remainingDistance)
    {
        speedFactor = Mathf.Clamp01(remainingDistance / slowDownThreshold);
        speedFactor = Mathf.Max(speedFactor, enemy.minSpeedFactor);

        enemy.controller.Move(speed * speedFactor * Time.deltaTime * direction);
        enemy.navMeshAgent.nextPosition = enemy.transform.position;
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
                enemy.moveAmount = 1.5f * speedFactor;
            }

            enemy.enemyAnimatorManager.SetAnimatorParameters(0, enemy.moveAmount, true);
        }
        else
        {
            if (enemy.characterStateMachine.currentState == enemy.strafeState)
            {
                enemy.enemyAnimatorManager.SetAnimatorParameters(horizontalInput, 0, true);
            }
            else
            {
                enemy.enemyAnimatorManager.SetAnimatorParameters(-horizontalInput, verticalInput, true);
            }
        }
    }

    protected override void HandleRotation()
    {
        if (!enemy.canRotate) return;

        lookDirection = enemy.navMeshAgent.destination - enemy.transform.position;
        lookDirection.y = 0;

        if (lookDirection == Vector3.zero) return;

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
using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    Enemy enemy;

    Vector3 desiredVelocity;
    Vector3 lookDirection;

    Quaternion lookRotation;

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

    protected override void HandleGroundedMovement()
    {
        if (!enemy.canMove)
            return;

        desiredVelocity = enemy.navMeshAgent.desiredVelocity;

        if (enemy.characterStateMachine.currentState == enemy.sprintState)
        {
            // sprint speed
            enemy.controller.Move(enemy.sprintSpeed * Time.deltaTime * desiredVelocity.normalized);
        }
        else if (enemy.characterStateMachine.currentState == enemy.combatState)
        {
            // combat speed
            enemy.controller.Move(enemy.combatSpeed * Time.deltaTime * desiredVelocity.normalized);
        }
        else
        {
            if (enemy.moveAmount > 0.5f)
            {
                // running speed
                enemy.controller.Move(enemy.runningSpeed * Time.deltaTime * desiredVelocity.normalized);
            }
            else if (enemy.moveAmount <= 0.5f)
            {
                // walking speed
                enemy.controller.Move(enemy.walkingSpeed * Time.deltaTime * desiredVelocity.normalized);
            }
        }

        GetMovementInput();

        enemy.navMeshAgent.velocity = enemy.controller.velocity;
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

        if (!enemy.isLockedOn)
        {
            if (enemy.characterStateMachine.currentState == enemy.sprintState)
            {
                enemy.enemyAnimatorManager.SetAnimatorParameters(0, 1.5f);
            }
            else
            {
                enemy.enemyAnimatorManager.SetAnimatorParameters(0, enemy.moveAmount);
            }
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
}

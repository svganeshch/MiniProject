using UnityEngine;

public class StrafeState : State
{
    bool strafeSwitch = false;
    bool tryAttack = false;

    float strafeTimer = 0f;
    float strafeSwitchTimer = 0f;
    int strafeDirection = 1;

    float attackRange = 2f;
    float attackProbability = 0.5f;

    public StrafeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.canMove = false;
        enemy.isLockedOn = true;
        strafeSwitch = false;
        tryAttack = false;

        strafeTimer = 0f;
        strafeSwitchTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckDistance();
        HandleStrafe();
        TryAttack();

        enemy.navMeshAgent.nextPosition = enemy.transform.position;
    }

    private void HandleStrafe()
    {
        if (tryAttack) return;

        if (strafeSwitch)
        {
            //CheckDistance();
            strafeSwitchTimer += Time.deltaTime;

            if (Random.value <= attackProbability)
                tryAttack = true;
        }

        if (strafeSwitchTimer >= enemy.strafeSwitchDuration)
        {
            strafeSwitchTimer = 0f;
            strafeSwitch = false;
        }

        if (!strafeSwitch)
        {
            //CheckDistance();

            strafeTimer += Time.deltaTime;

            if (strafeTimer >= enemy.strafeDuration)
            {
                strafeDirection *= -1;
                strafeTimer = 0f;

                strafeSwitch = true;
            }

            Vector3 strafeDirectionVector = enemy.transform.right * strafeDirection;
            strafeDirectionVector.y = 0f;
            strafeDirectionVector.Normalize();

            enemy.enemyAnimatorManager.SetAnimatorParameters(strafeDirection * enemy.strafeSpeed, 0);
            enemy.controller.Move(enemy.strafeSpeed * Time.deltaTime * strafeDirectionVector);
        }
        else
        {
            enemy.enemyAnimatorManager.SetAnimatorParameters(0, 0);
            enemy.moveAmount = 0;
        }

        enemy.navMeshAgent.velocity = enemy.controller.velocity;
    }

    private void TryAttack()
    {
        if (!tryAttack) return;

        if (enemy.navMeshAgent.remainingDistance > attackRange)
        {
            Vector3 attackDirection = enemy.currentTarget.transform.position - enemy.transform.position;
            attackDirection.y = 0f;
            attackDirection.Normalize();

            enemy.controller.Move(enemy.walkingSpeed * Time.deltaTime * attackDirection);
            enemy.enemyAnimatorManager.SetAnimatorParameters(0, 1);
        }
        else
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        enemy.navMeshAgent.velocity = enemy.controller.velocity;
    }

    private void CheckDistance()
    {
        if (enemy.navMeshAgent.remainingDistance > enemy.navMeshAgent.stoppingDistance + 1)
        {
            stateMachine.ChangeState(enemy.combatState);
        }
        else if (enemy.navMeshAgent.remainingDistance < attackRange)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.canMove = true;
        enemy.isLockedOn = false;
    }
}
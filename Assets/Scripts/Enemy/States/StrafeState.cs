using UnityEngine;

public class StrafeState : State
{
    bool strafeSwitch = false;

    float strafeSwitchTimer = 0f;
    int strafeDirection = 1;

    Vector3 strafeStartPosition;

    public StrafeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.enemyMovementManager.manualUpdate = true;
        enemy.isLockedOn = true;
        strafeSwitch = false;

        strafeSwitchTimer = 0f;
        strafeStartPosition = enemy.transform.position;
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
        if (strafeSwitch)
        {
            strafeSwitchTimer += Time.deltaTime;

            if (Random.value <= enemy.attackProbability)
                stateMachine.ChangeState(enemy.attackState);
        }

        if (strafeSwitchTimer >= enemy.strafeSwitchDuration)
        {
            strafeSwitchTimer = 0f;
            strafeSwitch = false;
            strafeStartPosition = enemy.transform.position;
        }

        if (!strafeSwitch)
        {
            float distanceMoved = Vector3.Distance(strafeStartPosition, enemy.transform.position);
            float remainingDistance = enemy.strafeDistance - distanceMoved;

            if (distanceMoved >= enemy.strafeDistance)
            {
                strafeDirection *= -1;
                strafeStartPosition = enemy.transform.position;
                strafeSwitch = true;
            }
            else
            {
                Vector3 strafeDirectionVector = enemy.transform.right * strafeDirection;
                strafeDirectionVector.y = 0f;
                strafeDirectionVector.Normalize();

                enemy.enemyMovementManager.UpdateMovement(enemy.strafeSpeed, strafeDirectionVector, remainingDistance);
            }
        }
    }


    private void CheckDistance()
    {
        if (enemy.navMeshAgent.remainingDistance > enemy.navMeshAgent.stoppingDistance + 1)
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
}
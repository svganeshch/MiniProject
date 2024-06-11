using UnityEngine;

public class StrafeState : State
{
    bool strafeSwitch = false;

    float strafeTimer = 0f;
    float strafeSwitchTimer = 0f;
    float strafeDuration = 8f;
    float strafeSwitchDuration = 10f;
    int strafeDirection = 1;

    public StrafeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.isLockedOn = true;
        strafeSwitch = false;

        strafeTimer = 0f;
        strafeSwitchTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckDistance();
        enemy.CheckRecallDistance();

        if (strafeSwitch) strafeSwitchTimer += Time.deltaTime;

        if (strafeSwitchTimer >= strafeSwitchDuration)
        {
            strafeSwitchTimer = 0f;
            strafeSwitch = false;
        }

        if (!strafeSwitch)
        {
            strafeTimer += Time.deltaTime;

            if (strafeTimer >= strafeDuration)
            {
                strafeDirection *= -1;
                strafeTimer = 0f;

                strafeSwitch = true;
            }

            Vector3 strafeDirectionVector = enemy.transform.right * strafeDirection;
            strafeDirectionVector.y = 0f;
            strafeDirectionVector.Normalize();

            enemy.controller.Move(enemy.strafeSpeed * Time.deltaTime * strafeDirectionVector);

            enemy.enemyAnimatorManager.SetAnimatorParameters(strafeDirection * enemy.strafeSpeed, 0);
        }
        else
        {
            enemy.enemyAnimatorManager.SetAnimatorParameters(0, 0);
            enemy.moveAmount = 0;
        }
    }

    private void CheckDistance()
    {
        enemy.navMeshAgent.destination = enemy.currentTarget.transform.position;

        if (enemy.navMeshAgent.remainingDistance > enemy.navMeshAgent.stoppingDistance + 1)
        {
            stateMachine.ChangeState(enemy.combatState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.isLockedOn = false;
    }
}

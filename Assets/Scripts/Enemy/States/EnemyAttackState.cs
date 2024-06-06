using UnityEngine;

public class EnemyAttackState : State
{
    private bool canAttack = false;
    private bool canCombo = false;
    private float chaseDistanceTreshold = 0.75f;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;
    private float clipTime;

    public EnemyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canAttack = false;
        timePassed = 0;

        enemy.enemyAnimatorManager.PlayLiteAttackAction(canCombo);
        canCombo = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timePassed += Time.deltaTime;

        LookAtTarget();
        canAttack = CheckAttackDistance();

        Debug.Log(character.animator.GetCurrentAnimatorClipInfo(3)[0].clip.name);

        clipLength = character.animator.GetCurrentAnimatorClipInfo(3)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(3).speed * character.animator.GetCurrentAnimatorStateInfo(0).speedMultiplier;
        clipTime = clipLength / clipSpeed;

        if (timePassed >= clipTime)
        {
            if (canAttack)
            {
                //Debug.Log("switching state to next attack");
                stateMachine.ChangeState(enemy.attackState, true);
                canCombo = true;
            }
            else
            {
                Debug.Log("out of radius");
                stateMachine.ChangeState(enemy.combatState);
            }
        }
    }

    private void LookAtTarget()
    {
        Vector3 targetDirection = enemy.currentTarget.transform.position - enemy.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
        {
            targetDirection = enemy.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, 0.5f);
    }

    private bool CheckAttackDistance()
    {
        float remainingDistance = Vector3.Distance(enemy.transform.position, enemy.currentTarget.transform.position);
        return (remainingDistance < enemy.navMeshAgent.stoppingDistance + chaseDistanceTreshold);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

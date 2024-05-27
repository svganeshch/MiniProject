using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool canAttack = false;
    private float chaseDistanceTreshold = 0.75f;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;
    private float clipTime;

    public EnemyAttackState(Enemy _enemy, EnemyStateMachine _enemyStateMachine) : base(_enemy, _enemyStateMachine)
    {
        enemy = _enemy;
        enemyStateMachine = _enemyStateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        canAttack = false;
        timePassed = 0;

        enemy.animator.applyRootMotion = true;
        enemy.animator.SetFloat("speed", 0);
        enemy.animator.SetTrigger("attack");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timePassed += Time.deltaTime;

        LookAtTarget();
        canAttack = CheckAttackDistance();

        //Debug.Log(enemy.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);

        clipLength = enemy.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        clipSpeed = enemy.animator.GetCurrentAnimatorStateInfo(0).speed * enemy.animator.GetCurrentAnimatorStateInfo(0).speedMultiplier;
        clipTime = clipLength / clipSpeed;

        if (timePassed >= clipTime)
        {
            if (canAttack)
            {
                //Debug.Log("switching state to next attack");
                enemyStateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                //Debug.Log("out of radius");
                enemy.animator.SetTrigger("move");
                enemyStateMachine.ChangeState(enemy.pursueState);
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
        return (remainingDistance < enemy.navMesh.stoppingDistance + chaseDistanceTreshold);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.animator.applyRootMotion = false;
    }
}

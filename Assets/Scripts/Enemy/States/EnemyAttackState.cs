using UnityEngine;

public class EnemyAttackState : State
{
    private float comboProbability = 0.25f;
    private bool canAttack;
    private bool isCombo;

    private Vector3 startPos;

    public EnemyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canAttack = false;
        isCombo = false;
        enemy.attackCoolDownTimer = 0;

        startPos = enemy.transform.position;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        TryAttack();

        if (!canAttack) return;

        if (Random.value <= comboProbability)
            isCombo = true;

        if (!enemy.isAttacking && !enemy.isInCoolDown)
        {
            canAttack = false;
            enemy.isInCoolDown = true;

            if (isCombo)
            {
                isCombo = false;
                enemy.enemyAnimatorManager.PlayLiteAttackAction(true, true);
            }
            else
            {
                enemy.enemyAnimatorManager.PlayLiteAttackAction(false, true);
            }
        }
    }

    private void TryAttack()
    {
        if (enemy.navMeshAgent.remainingDistance > enemy.attackRange)
        {
            Vector3 attackDirection = enemy.currentTarget.transform.position - enemy.transform.position;
            attackDirection.y = 0f;
            attackDirection.Normalize();

            enemy.enemyMovementManager.UpdateMovement(enemy.walkingSpeed, attackDirection, 5);
        }
        else
        {
            canAttack = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

using UnityEngine;

public class EnemyAttackState : State
{
    private float comboProbability = 0;
    private int comboAttackCount = 3;
    private int count = 1;

    private bool canAttack;
    private bool isCombo;

    private Vector3 startPos;

    public EnemyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //character.weaponEquipment.GetCurrentWeapon().weaponAttackScript.OnStartDamageEvent.AddListener(SetDamageState);
        //character.weaponEquipment.GetCurrentWeapon().weaponAttackScript.OnStopDamageEvent.AddListener(ResetDamageState);

        if (enemy.isInstantAttack)
        {
            comboProbability = 1;
            enemy.isInstantAttack = false;
        }
        else
        {
            comboProbability = enemy.attackComboProbability;
        }

        canAttack = false;
        isCombo = false;

        count = 1;
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

        if (count <= comboAttackCount)
        {
            if (character.canCombo)
            {
                if (isCombo)
                {
                    character.canCombo = false;
                    isCombo = false;
                    enemy.enemyAnimatorManager.PlayLiteAttackAction(true, true);

                    count++;

                    return;
                }
            }
        }

        if (!enemy.isAttacking && (!enemy.isInCoolDown || enemy.isInstantAttack))
        {
            canAttack = false;
            enemy.isInCoolDown = true;

            enemy.enemyAnimatorManager.PlayLiteAttackAction(false, true);
        }
    }

    private void TryAttack()
    {
        if (enemy.navMeshAgent.remainingDistance > enemy.instantAttackRange)
        {
            Vector3 attackDirection = enemy.currentTarget.transform.position - enemy.transform.position;
            attackDirection.y = 0f;
            attackDirection.Normalize();

            enemy.enemyMovementManager.UpdateMovement(enemy.attackSpeed, attackDirection, enemy.navMeshAgent.remainingDistance);
        }
        else
        {
            canAttack = true;
        }
    }

    private void SetDamageState()
    {
        enemy.characterRigController.SetRigWeight(1f);
        enemy.characterRigController.SetRigTarget();
    }

    private void ResetDamageState()
    {
        enemy.characterRigController.SetRigWeight(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

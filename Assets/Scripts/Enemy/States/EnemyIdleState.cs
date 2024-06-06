using UnityEngine;

public class EnemyIdleState : State
{
    int destinationPoint = 0;

    public float viewableAngle;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    public EnemyIdleState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        destinationPoint = 0;
        enemy.moveAmount = 0.5f;

        enemy.navMeshAgent.autoBraking = false;
        SetPatrolPoint();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Patrol();
        FindTarget();

        if (enemy.currentTarget)
        {
            enemy.moveAmount = 1.0f;

            if (enemy.weaponEquipment.SetWeapon(defaultWeaponSlot))
            {
                enemy.enemyAnimatorManager.PlayWeaponDrawAction();
            }

            stateMachine.ChangeState(enemy.combatState);
        }
    }

    private void Patrol()
    {
        if (!enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)
        {
            SetPatrolPoint();
        }
    }

    private void SetPatrolPoint()
    {
        if (enemy.patrolPoints.Length == 0)
        {
            return;
        }

        enemy.navMeshAgent.destination = enemy.patrolPoints[destinationPoint].position;
        destinationPoint = (destinationPoint + 1) % enemy.patrolPoints.Length;
    }

    private void FindTarget()
    {
        if (enemy.isDead)
            return;

        Collider[] colliders = Physics.OverlapSphere(enemy.transform.position,
                                                     enemy.detectionRadius,
                                                     LayerMaskManager.Instance.playerLayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].transform.TryGetComponent<Player>(out var targetCharacter))
                continue;

            Vector3 targetDirection = targetCharacter.transform.position - enemy.transform.position;
            float angleOfTarget = Vector3.Angle(targetDirection, enemy.transform.forward);

            if (angleOfTarget > minimumFOV && angleOfTarget < maximumFOV)
            {
                if (Physics.Linecast(enemy.targetLock.position, targetCharacter.targetLockCast.position, LayerMaskManager.Instance.obstaclesLayerMask))
                {
                    continue;
                }
                else
                {
                    //targetDirection = targetCharacter.transform.position - enemy.transform.position;
                    //viewableAngle = enemy.pursueState.GetAngleOfTarget(enemy.transform, enemy.idleState.targetDirection);
                    enemy.currentTarget = targetCharacter;
                    //PivotTowardsTarget();
                    Debug.Log("target found");
                }
            }
        }
    }

    public void PivotTowardsTarget()
    {
        if (viewableAngle >= 20 && viewableAngle <= 60)
        {
            enemy.animator.SetTrigger("R45");
        }
        else if (viewableAngle <= -20 && viewableAngle >= -60)
        {
            enemy.animator.SetTrigger("L45");
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.navMeshAgent.autoBraking = true;
    }
}

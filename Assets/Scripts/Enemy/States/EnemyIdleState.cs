using UnityEngine;

public class EnemyIdleState : State
{
    private int destinationPoint;

    private LayerMask playerLayerMask;
    private LayerMask obstaclesLayerMask;

    public EnemyIdleState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerLayerMask = LayerMaskManager.Instance.playerLayerMask;
        obstaclesLayerMask = LayerMaskManager.Instance.obstaclesLayerMask;

        destinationPoint = 0;

        SetNextPatrolPoint();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Patrol();
        DetectTarget();
    }

    private void Patrol()
    {
        if (!enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance + 0.2f)
        {
            SetNextPatrolPoint();
        }
    }

    private void SetNextPatrolPoint()
    {
        if (enemy.patrolPoints.Count == 0) return;

        enemy.navMeshAgent.destination = enemy.patrolPoints[destinationPoint];
        destinationPoint = (destinationPoint + 1) % enemy.patrolPoints.Count;
    }

    private void DetectTarget()
    {
        if (enemy.isDead) return;

        Collider[] colliders = new Collider[1];
        int colliderCount = Physics.OverlapSphereNonAlloc(
            enemy.transform.position,
            enemy.detectionRadius,
            colliders,
            playerLayerMask
        );

        for (int i = 0; i < colliderCount; i++)
        {
            if (!colliders[i].transform.TryGetComponent(out Player targetCharacter)) continue;

            Vector3 targetDirection = targetCharacter.transform.position - enemy.transform.position;
            float angleToTarget = Vector3.Angle(targetDirection, enemy.transform.forward);

            if (angleToTarget > enemy.minimumFOV && angleToTarget < enemy.maximumFOV)
            {
                if (!Physics.Linecast(enemy.targetLock.position, targetCharacter.targetLockCast.position, obstaclesLayerMask))
                {
                    enemy.currentTarget = targetCharacter;
                    Debug.Log("Target found");

                    if (enemy.weaponEquipment.SetWeapon(defaultWeaponSlot))
                    {
                        enemy.enemyAnimatorManager.PlayWeaponDrawAction();
                    }

                    stateMachine.ChangeState(enemy.combatState);
                    return;
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
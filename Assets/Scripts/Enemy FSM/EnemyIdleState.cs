using UnityEngine;

public class EnemyIdleState : State
{
    public float viewableAngle;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    bool drawWeapon = false;

    public EnemyIdleState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void LogicUpdate()
    {
        if (drawWeapon)
        {
            drawWeapon = false;

            if (character.weaponEquipment.SetWeapon(defaultWeaponSlot))
            {
                character.animator.SetTrigger("drawWeapon");
                stateMachine.ChangeState(character.combatState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        if (enemy.currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            drawWeapon = true;
        }
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
}

using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public Vector3 targetDirection;
    public float viewableAngle;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    public EnemyIdleState(Enemy _enemy, EnemyStateMachine _enemyStateMachine) : base(_enemy, _enemyStateMachine)
    {
        enemy = _enemy;
        enemyStateMachine = _enemyStateMachine;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            enemyStateMachine.ChangeState(enemy.pursueState);
        }
    }

    private void FindTarget()
    {
        if (enemy.isDead)
            return;

        Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, enemy.detectionRadius, Character.instance.playerLayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Character targetCharacter = colliders[i].transform.GetComponent<Character>();

            if (targetCharacter == null)
                continue;

            Vector3 targetDirection = targetCharacter.transform.position - enemy.transform.position;
            float angleOfTarget = Vector3.Angle(targetDirection, enemy.transform.forward);

            if (angleOfTarget > minimumFOV && angleOfTarget < maximumFOV)
            {
                if (Physics.Linecast(enemy.targetLock.position, targetCharacter.targetLockCast.position, Character.instance.obstaclesLayerMask))
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

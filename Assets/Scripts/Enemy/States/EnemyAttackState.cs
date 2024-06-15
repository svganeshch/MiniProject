using UnityEngine;

public class EnemyAttackState : State
{
    private bool isCombo = false;
    private float comboProbability = 0.15f;

    public EnemyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.enemyAnimatorManager.PlayLiteAttackAction(isCombo, true);
        isCombo = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.canCombo)
        {
            if (Random.value <= comboProbability)
                isCombo = true;
        }

        if (isCombo)
        {
            stateMachine.ChangeState(this, true);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

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

    int animatorActionsLayerindex;

    public EnemyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canAttack = false;
        timePassed = 0;

        animatorActionsLayerindex = enemy.animator.GetLayerIndex("Action Override");

        enemy.enemyAnimatorManager.PlayLiteAttackAction(canCombo, true);
        canCombo = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        canAttack = CheckAttackDistance();

        if (enemy.animator.GetCurrentAnimatorClipInfo(animatorActionsLayerindex).Length == 0) return;
        clipTime = enemy.animator.GetCurrentAnimatorStateInfo(animatorActionsLayerindex).normalizedTime;
        Debug.Log(enemy.animator.GetCurrentAnimatorClipInfo(animatorActionsLayerindex)[0].clip.name);

        timePassed += Time.deltaTime;

        //clipLength = character.animator.GetCurrentAnimatorClipInfo(animatorActionsLayerindex)[0].clip.length;
        //clipSpeed = character.animator.GetCurrentAnimatorStateInfo(animatorActionsLayerindex).speed * character.animator.GetCurrentAnimatorStateInfo(0).speedMultiplier;
        //clipTime = clipLength / clipSpeed;
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

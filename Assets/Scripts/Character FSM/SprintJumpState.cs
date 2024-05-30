using UnityEngine;

public class SprintJumpState : State
{
    float timePassed;
    float jumpTime;

    public SprintJumpState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();
        character.animator.applyRootMotion = true;
        timePassed = 0f;
        character.animator.SetTrigger("sprintJump");

        jumpTime = 0.75f;
    }

    public override void LogicUpdate()
    {

        base.LogicUpdate();
        if (timePassed > jumpTime)
        {
            character.animator.SetTrigger("move");
            stateMachine.ChangeState(player.sprintState);
        }
        timePassed += Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();
        character.animator.applyRootMotion = false;
    }
}

using UnityEngine;

public class LandState : State
{
    float timePassed;
    float landingTime;

    public LandState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();
        timePassed = 0f;
        character.animator.SetTrigger("land");
        landingTime = 0.5f;
    }

    public override void LogicUpdate()
    {

        base.LogicUpdate();
        if (timePassed > landingTime)
        {
            character.animator.SetTrigger("move");
            stateMachine.ChangeState(player.idleState);
        }
        timePassed += Time.deltaTime;
    }



}

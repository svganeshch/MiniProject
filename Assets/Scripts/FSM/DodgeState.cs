using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : State
{
    Vector3 rollDirection;

    float timePassed;
    float dodgeTime;

    public DodgeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        character.animator.applyRootMotion = true;

        if (character.animator.GetFloat("speedY") >= 0.5f)
            character.animator.SetTrigger("dodge");
        else
            if (character.animator.GetBool("isCombat"))
            stateMachine.ChangeState(character.combatState);
        else
            stateMachine.ChangeState(character.idleState);

        input = Vector2.zero;
        timePassed = 0;
        dodgeTime = 0.8f;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        rollDirection = PlayerCamera.instance.cameraObj.transform.forward * verticalInput;
        rollDirection += PlayerCamera.instance.cameraObj.transform.right * horizontalInput;
        rollDirection.y = 0;
        rollDirection.Normalize();

        Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
        character.transform.rotation = playerRotation;

        if (timePassed > dodgeTime)
        {
            character.animator.SetTrigger("move");

            if (character.animator.GetBool("isCombat"))
                stateMachine.ChangeState(character.combatState);
            else
                stateMachine.ChangeState(character.idleState);
        }
        timePassed += Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}

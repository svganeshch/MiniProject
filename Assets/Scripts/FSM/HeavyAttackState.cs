using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttackState : State
{
    bool attack;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;

    public HeavyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        attack = false;
        character.animator.applyRootMotion = true;
        timePassed = 0f;

        character.animator.SetBool("isCombat", true);
        character.animator.SetTrigger("heavyAttack");
        character.animator.SetFloat("speedY", 0f);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            character.animator.SetTrigger("dodge");
            stateMachine.ChangeState(character.combatState);
        }

        if (heavyAttackWeaponAction.triggered)
        {
            attack = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timePassed += Time.deltaTime;

        //Debug.Log(character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.name);

        clipLength = character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(1).speed * character.animator.GetCurrentAnimatorStateInfo(1).speedMultiplier;

        if (timePassed >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(character.heavyAttackState);
        }
        if (timePassed >= clipLength / clipSpeed)
        {
            character.animator.SetTrigger("move");
            stateMachine.ChangeState(character.combatState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        character.animator.applyRootMotion = false;
    }
}

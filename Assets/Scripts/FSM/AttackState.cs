using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    bool attack;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;

    private int weaponLayerIndex;

    public AttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
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
        character.animator.SetTrigger("attack");
        character.animator.SetFloat("speed", 0f);

        weaponLayerIndex = character.weaponEquipment.GetCurrentWeapon().weaponAnimLayerIndex;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (attackWeaponAction.triggered)
        {
            attack = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timePassed += Time.deltaTime;

        Debug.Log(character.animator.GetCurrentAnimatorClipInfo(weaponLayerIndex)[0].clip.name);

        clipLength = character.animator.GetCurrentAnimatorClipInfo(weaponLayerIndex)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(weaponLayerIndex).speed;

        if (timePassed >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(character.attackState);
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

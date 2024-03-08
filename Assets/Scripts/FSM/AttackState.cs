using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    bool attack;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;

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

        character.animator.SetBool("isCombat", true);
        character.animator.SetTrigger("attack");
        character.animator.SetFloat("speed", 0f);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            character.animator.SetTrigger("dodge");
            stateMachine.ChangeState(character.combatState);
        }

        if (attackWeaponAction.triggered)
        {
            attack = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timePassed += Time.deltaTime;

        character.transform.rotation = Quaternion.Euler(0f, character.cameraTransform.eulerAngles.y, 0f);

        CheckEnemy();
        Debug.Log(character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.name);

        clipLength = character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(1).speed * character.animator.GetCurrentAnimatorStateInfo(1).speedMultiplier;

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

    private void CheckEnemy()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(character.transform.position, 2.5f, character.enemyLayerMask);

        foreach (var enemy in enemyColliders)
        {
            character.transform.LookAt(enemy.transform);
            Debug.Log(enemy.transform.position);
        }
    }

    public override void Exit()
    {
        base.Exit();
        character.animator.applyRootMotion = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(character.transform.position, 2.5f);
    }
}

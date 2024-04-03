using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteAttackState : AttackStateLogic
{
    public LiteAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        character.animator.SetTrigger("liteAttack");
    }
}

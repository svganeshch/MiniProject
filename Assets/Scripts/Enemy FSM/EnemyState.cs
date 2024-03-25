using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    public EnemyState(Enemy _enemy, EnemyStateMachine _enemyStateMachine)
    {
        enemy = _enemy;
        enemyStateMachine = _enemyStateMachine;
    }

    public virtual void Enter()
    {
        //Debug.Log("Entered state " + this.ToString());
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Exit()
    {
        //Debug.Log("Exited state " + this.ToString());
    }
}

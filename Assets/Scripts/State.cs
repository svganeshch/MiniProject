using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class State
{
    protected Character character;
    protected StateMachine stateMachine;

    protected Vector2 input;
    protected Vector3 velocity;
    protected Vector3 gravityVelocity;

    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction sprintAction;

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        moveAction = character.playerInput.actions["Move"];
        jumpAction = character.playerInput.actions["Jump"];
        sprintAction = character.playerInput.actions["Sprint"];
    }

    public virtual void Enter()
    {
        Debug.Log("Entered state " + this.ToString());
    }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit()
    {
        Debug.Log("Exited state " + this.ToString());
    }
}

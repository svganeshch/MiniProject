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
    public InputAction dodgeAction;

    public InputAction drawWeaponAction;
    public InputAction attackWeaponAction;
    public InputAction heavyAttackWeaponAction;
    public InputAction lockOnAction;

    public InputAction weapon1Action;
    public InputAction weapon2Action;
    public InputAction weapon3Action;
    public InputAction weapon4Action;

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        moveAction = character.playerInput.actions["Move"];
        jumpAction = character.playerInput.actions["Jump"];
        sprintAction = character.playerInput.actions["Sprint"];
        dodgeAction = character.playerInput.actions["Dodge"];
        drawWeaponAction = character.playerInput.actions["DrawWeapon"];
        attackWeaponAction = character.playerInput.actions["Attack"];
        heavyAttackWeaponAction = character.playerInput.actions["HeavyAttack"];
        lockOnAction = character.playerInput.actions["LockOn"];

        weapon1Action = character.playerInput.actions["Weapon1"];
        weapon2Action = character.playerInput.actions["Weapon2"];
        weapon3Action = character.playerInput.actions["Weapon3"];
        weapon4Action = character.playerInput.actions["Weapon4"];
    }

    public virtual void Enter()
    {
        //Debug.Log("Entered state " + this.ToString());
    }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit()
    {
        //Debug.Log("Exited state " + this.ToString());
    }
}

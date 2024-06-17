using UnityEngine;

public class State
{
    protected Character character;
    protected Player player;
    protected Enemy enemy;
    protected StateMachine stateMachine;

    // Swap weapon variables
    public bool isSwappingWeapon = false;
    public int defaultWeaponSlot = 1;
    public int previousWeaponSlot = 0;

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        if (_character is Player p)
        {
            player = p;
        }
        else if (_character is Enemy e)
        {
            enemy = e;
        }
    }

    public virtual void Enter()
    {
        // Debug.Log("Entered state " + character.name + " : " + this);
    }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit()
    {
        // Debug.Log("Exited state " + character.name + " : " + this);
    }

    public virtual void SetTarget(Enemy target) { }

    public virtual void SwapWeapon() { }

    public Vector3 GetCharacterDirection()
    {
        Vector3 characterDirection = PlayerCamera.Instance.playerCameraObjTransform.forward * player.verticalInput;
        characterDirection += PlayerCamera.Instance.playerCameraObjTransform.right * player.horizontalInput;
        characterDirection.y = 0;

        return characterDirection.normalized;
    }

    public void HandleRotation()
    {
        if (player.isLockedOn)
        {
            HandleLockedOnRotation();
        }
        else
        {
            HandleFreeRotation();
        }
    }

    private void HandleLockedOnRotation()
    {
        if (player.currentLockedOnTarget == null)
        {
            return;
        }

        Vector3 lockedTargetDirection = player.currentLockedOnTarget.transform.position - player.transform.position;
        lockedTargetDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lockedTargetDirection.normalized);
        player.transform.rotation = targetRotation;
    }

    private void HandleFreeRotation()
    {
        Vector3 targetRotationDirection = PlayerCamera.Instance.mainCameraTransform.forward * player.verticalInput;
        targetRotationDirection += PlayerCamera.Instance.mainCameraTransform.right * player.horizontalInput;
        targetRotationDirection.y = 0f;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = player.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetRotationDirection.normalized);
        player.transform.rotation = targetRotation;
    }
}
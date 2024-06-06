using UnityEngine;

public class State
{
    protected Character character;
    protected Player player;
    protected Enemy enemy;
    protected StateMachine stateMachine;

    private Vector3 characterDirection;
    private Vector3 lockedTargetDirection;
    private Vector3 targetRotationDirection;
    private Quaternion targetRotation;

    // Swap weapon vars
    public int defaultWeaponSlot = 1;
    public int previousWeaponSlot = 0;
    public virtual void SwapWeapon() { }

    // Player lockon
    public virtual void SetTarget(Enemy target) { }

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        switch (_character)
        {
            case Player:
                player = _character as Player;
                break;
            case Enemy:
                enemy = _character as Enemy;
                break;
        }
    }

    public virtual void Enter()
    {
        //Debug.Log("Entered state " + this.ToString());
    }

    public virtual void HandleInput()
    {

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

    public Vector3 GetCharacterDirection()
    {
        characterDirection = PlayerCamera.Instance.transform.forward * player.verticalInput;
        characterDirection += PlayerCamera.Instance.transform.right * player.horizontalInput;
        characterDirection.y = 0;

        return characterDirection;
    }

    public void HandleRotation()
    {
        if (player.isLockedOn)
        {
            if (!player.currentLockedOnTarget)
                return;

            lockedTargetDirection = Vector3.zero;
            lockedTargetDirection = player.currentLockedOnTarget.transform.position - player.transform.position;
            lockedTargetDirection.y = 0f;
            lockedTargetDirection.Normalize();

            targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            player.transform.rotation = targetRotation;
        }
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.Instance.cameraObj.transform.forward * player.verticalInput;
            targetRotationDirection += PlayerCamera.Instance.cameraObj.transform.right * player.horizontalInput;
            targetRotationDirection.y = 0f;
            targetRotationDirection.Normalize();

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = player.transform.forward;
            }

            targetRotation = Quaternion.LookRotation(targetRotationDirection);
            player.transform.rotation = targetRotation;
        }
    }
}

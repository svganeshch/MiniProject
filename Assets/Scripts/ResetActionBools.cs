using UnityEngine;

public class ResetActionBools : StateMachineBehaviour
{
    Character character;
    WeaponEquipment weapon;
    CharacterAnimatorManager animatorManager;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<Character>();
            weapon = animator.GetComponent<WeaponEquipment>();
            animatorManager = animator.GetComponent<CharacterAnimatorManager>();
        }

        character.applyRootMotion = false;
        character.isPivoting = false;
        character.isAttacking = false;
        character.canRotate = true;
        character.canMove = true;

        if (character.characterAnimatorManager.CombatBool)
        {
            character.characterStateMachine.ChangeState(character.combatState);
        }
        else
        {
            character.characterStateMachine.ChangeState(character.idleState);
        }

        if (!weapon && !animatorManager)
        {
            weapon.StopDamage();
            animatorManager.DisablePerformAction();
            animatorManager.DisableCombo();
        }
    }
}

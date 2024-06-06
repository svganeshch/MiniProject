using UnityEngine;

public class ResetActionBools : StateMachineBehaviour
{
    Character character;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<Character>();
        }

        character.applyRootMotion = false;
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
    }
}

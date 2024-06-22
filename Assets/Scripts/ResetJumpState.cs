using UnityEngine;

public class ResetJumpState : StateMachineBehaviour
{
    Character character;
    Player player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<Character>();
            player = animator.GetComponent<Player>();
        }

        if (character.characterStateMachine.currentState != null)
        {
            if (character.characterStateMachine.currentState == character.jumpState)
            {
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
    }
}

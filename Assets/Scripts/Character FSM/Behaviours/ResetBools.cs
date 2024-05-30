using UnityEngine;

public class ResetBools : StateMachineBehaviour
{
    Character character;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<Character>();
        }

        character.hitState.hitDone = true;
        character.blockBrokenState.blockBrokenDone = true;

        Player.Instance.dodgeState.dodgeDone = true;
    }
}

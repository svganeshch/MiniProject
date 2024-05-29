using UnityEngine;

public class ResetBools : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Character.instance.hitState.hitDone = true;

        Character.instance.dodgeState.dodgeDone = true;

        Character.instance.blockBrokenState.blockBrokenDone = true;
    }
}

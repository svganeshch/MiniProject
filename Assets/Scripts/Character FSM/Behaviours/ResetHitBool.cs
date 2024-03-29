using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHitBool : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Character.instance.hitState.hitDone = true;
    }
}

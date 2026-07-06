using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateHarvester : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Harvester harvester = animator.GetComponent<Harvester>();
        harvester.assignedNode = null;
    }

    /*override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }*/
}

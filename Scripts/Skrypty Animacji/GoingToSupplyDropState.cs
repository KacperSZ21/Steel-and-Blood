using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingToSupplyDropState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Harvester harvester = animator.GetComponent<Harvester>();
        if (harvester.supplyCenter != null)
        {
            harvester.MoveTo(harvester.supplyCenter);
        }
        else
        {
            harvester.Cancelharvesting();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {

    // }
}

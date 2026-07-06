using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitIdleState : StateMachineBehaviour
{
    Attackcontroller attackController;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<Attackcontroller>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Sprawdz czy wrog w poblizu
        if (attackController.targettoAttack != null)
        {
            //Zmiana na podazanie
            animator.SetBool("isFollowing", true);
        }
    }
}

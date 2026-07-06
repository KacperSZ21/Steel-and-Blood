using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    public float attackingDistance;

    Attackcontroller attackController;
    NavMeshAgent agent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<Attackcontroller>();
        agent = animator.transform.GetComponent<NavMeshAgent>();

        if (attackController.enemyBuilding == true)
        {
            attackingDistance += 4f;
        }
        else
        {
            attackingDistance = 1.2f;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Zmiana na spokojny state
        if (attackController.targettoAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            //Jezeli nie ma rozkazu od gracza do poruszenia
            if (animator.transform.GetComponent<UnitMovement>().isCommandedtoMove == false)
            {
                //Podozanie
                agent.SetDestination(attackController.targettoAttack.position);
                animator.transform.LookAt(attackController.targettoAttack);

                //Zmiana na atak state
                float distanceFromTarget = Vector3.Distance(attackController.targettoAttack.position, animator.transform.position);
                if (distanceFromTarget < attackingDistance)
                {
                    agent.SetDestination(animator.transform.position);
                    animator.SetBool("isAttacking", true);
                }
            }
        }

    }
}

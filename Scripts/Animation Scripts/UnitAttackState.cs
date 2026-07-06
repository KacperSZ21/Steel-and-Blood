using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;
using Vector3 = UnityEngine.Vector3;

public class UnitAttackState : StateMachineBehaviour
{
    public float stopAttackingDistance = 1.2f;
    public float attackRate = 2f;
    public float attackTimer;

    NavMeshAgent agent;
    Attackcontroller attackController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<Attackcontroller>();
        if (attackController.enemyBuilding == true)
        {
            stopAttackingDistance += 4f;
        }
        else
        {
            stopAttackingDistance = 1.2f;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.LookAt(attackController.targettoAttack);
        if (attackController.targettoAttack != null && animator.transform.GetComponent<UnitMovement>().isCommandedtoMove == false)
        {
            //Idz za celem
            //agent.SetDestination(attackController.targettoAttack.position);

            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = 1f / attackRate;

                if (UnityEngine.Random.value <= 0.2 && animator.GetBool("SpecialAttack") == false)
                {
                    animator.SetBool("SpecialAttack", true);
                }
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }

            //Czy powinien atakowac
            float distanceFromTarget = UnityEngine.Vector3.Distance(attackController.targettoAttack.position, animator.transform.position);
            if (distanceFromTarget > stopAttackingDistance || attackController.targettoAttack == null)
            {
                animator.SetBool("isAttacking", false);//Set Follow State
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);//Set Follow State
        }
    }

    private void Attack()
    {
        var damageToInflict = attackController.unitDamage;

        SoundManagerScript.Instance.PlayinfantryattackSound();

        //Atakowanie
        var damageable = attackController.targettoAttack.GetComponent<IDamgaeable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageToInflict);
        }
    }
}

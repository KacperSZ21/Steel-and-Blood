using UnityEngine;
using System.Collections;

public class TrackingPlayerUnits : MonoBehaviour
{
    private Attackcontroller attackController;
    private UnitMovement unitMovement;

    private bool isTracking = false;

    void Awake()
    {
        attackController = GetComponent<Attackcontroller>();
        unitMovement = GetComponent<UnitMovement>();
    }

    void Update()
    {
        // If we’re already tracking it → don’t restart it
        if (isTracking)
            return;

        // If the unit does NOT have a target → start checking
        if (attackController != null && attackController.targettoAttack == null)
        {
            StartCoroutine(CheckAfterStop());
        }
    }

    private IEnumerator CheckAfterStop()
    {
        isTracking = true;

        // Wait until the unit stops moving
        yield return new WaitUntil(() => !IsMoving());

        // If there is still no target → find a new one
        if (attackController.targettoAttack == null)
        {
            GameObject newTarget = attackController.FindingPlayerUnits();

            if (newTarget != null)
            {
                // set target
                attackController.targettoAttack = newTarget.transform;

                // go to him (false = this is not a player command)
                if (unitMovement != null)
                {
                    unitMovement.MoveTo(newTarget.transform.position, false);
                }
            }
        }

        isTracking = false;
    }

    // Checking whether the unit is moving
    private bool IsMoving()
    {
        if (unitMovement == null)
            return false;

        UnityEngine.AI.NavMeshAgent agent = unitMovement.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            return agent.velocity.magnitude > 0.1f;
        }

        return false;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    public LayerMask ground;
    public bool isCommandedtoMove;
    public bool allowManualInput = true;

    Camera cam;
    NavMeshAgent agent;
    DirectionIndicator line;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        cam = Camera.main;
        line = gameObject.GetComponent<DirectionIndicator>();
    }

    void Update()
    {
        if (!allowManualInput) return;

        if (Input.GetMouseButtonDown(1) && IsMovingPossible() && gameObject.CompareTag("Unit"))
        {
            //The sound of a command being given
            SoundManagerScript.Instance.PlayinfantryMoveSound();
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                MoveTo(hit.point, true); // player command = true

                line.DrawLine(hit);
            }
        }
    }

    private bool IsMovingPossible()
    {
        return CursorManager.Instance.currentCursor != CursorManager.CursorType.UnAvailable;
    }

    IEnumerator NoCommand()
    {
        yield return new WaitForSeconds(1);
        isCommandedtoMove = false;
    }

    public void MoveTo(Vector3 position, bool isPlayerCommand)
    {
        isCommandedtoMove = true;
        StartCoroutine(NoCommand());
        agent.SetDestination(position);

        if (isPlayerCommand)
        {
            Harvester harvester = GetComponent<Harvester>();
            if (harvester != null)
            {
                if (harvester.assignedNode != null)
                {
                    harvester.Cancelharvesting();
                }
            }
        }
    }
}

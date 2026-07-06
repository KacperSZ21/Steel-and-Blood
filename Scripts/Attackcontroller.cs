using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attackcontroller : MonoBehaviour
{
    public Transform targettoAttack;
    public int unitDamage;
    public bool isPlayer;
    public bool enemyBuilding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targettoAttack == null)
        {
            targettoAttack = other.transform;
        }
        else if (isPlayer && other.CompareTag("EnemyBuilding") && targettoAttack == null)
        {
            enemyBuilding = true;
            targettoAttack = other.transform;
        }
        else if (isPlayer == false && other.CompareTag("Unit") && targettoAttack == null)
        {
            targettoAttack = other.transform;
        }
        else if (isPlayer == false && other.CompareTag("Building") && targettoAttack == null)
        {
            enemyBuilding = true;
            targettoAttack = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targettoAttack == null)
        {
            targettoAttack = other.transform;
        }
        else if (isPlayer && other.CompareTag("EnemyBuilding") && targettoAttack == null)
        {
            enemyBuilding = true;
            targettoAttack = other.transform;
        }
        else if (isPlayer == false && other.CompareTag("Unit") && targettoAttack == null)
        {
            targettoAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targettoAttack != null)
        {
            enemyBuilding = false;
            targettoAttack = null;
        }
        else if (isPlayer && other.CompareTag("EnemyBuilding") && targettoAttack != null)
        {
            enemyBuilding = false;
            targettoAttack = null;
        }
        else if (isPlayer == false && other.CompareTag("Unit") && targettoAttack != null)
        {
            targettoAttack = null;
        }
    }

    private void OnDrawGizmos()
    {
        //Follow Distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f * 0.5f);

        //Attack Distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        //Stop Attack
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }

    public GameObject FindingPlayerUnits()
    {
        GameObject unit = null;
        int rng = (int)Random.Range(0, UnitSelectionManager.Instance.allUnitsList.Count);
        if (UnitSelectionManager.Instance.allUnitsList.Count > 0)
        {
            unit = UnitSelectionManager.Instance.allUnitsList[rng];
        }
        return unit;
    }

    public void IncreaseDamage(int amount)
    {
        unitDamage += amount;
    }
}

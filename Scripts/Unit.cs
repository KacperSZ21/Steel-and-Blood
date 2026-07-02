using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IDamgaeable
{
    public float unitMaxHealth;
    public UnitType unitType;
    public HealthTracker healthTracker;
    public GameObject healeffect;

    private float unitHealth;
    Animator animator;
    NavMeshAgent navMeshAgent;
    void Start()
    {
        if (!gameObject.CompareTag("Enemy"))
        {
            UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        }

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        healeffect = Resources.Load<GameObject>("HealEffect");
    }

    void Update()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    void OnDestroy()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
        }
    }
    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);
        if (unitHealth <= 0 && unitHealth > -400)
        {
            //Dying Logic
            gameObject.GetComponent<Unit>().enabled = false;
            gameObject.GetComponent<UnitMovement>().enabled = false;
            if (gameObject.GetComponent<Attackcontroller>() == null)
            {
                gameObject.GetComponent<Harvester>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<Attackcontroller>().unitDamage = 0;
                gameObject.GetComponent<Attackcontroller>().enabled = false;
                PopulationManagement.Instance.RemoveUnit(this);
            }
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            animator.SetTrigger("isDead");
            SoundManagerScript.Instance.PlayinfantryDeathSound();
            Destroy(gameObject, 1f);
            unitHealth = -500;
            return;
        }
    }

    public void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

    public void IncreaseHealth(float amount)
    {
        unitMaxHealth += amount;
        unitHealth += amount;
        UpdateHealthUI();
    }

    public float Gethealth()
    {
        return unitHealth;
    }

    public void HealUnit(float amount)
    {
        if (unitHealth + amount >= unitMaxHealth)
        {
            unitHealth = unitMaxHealth;
            Quaternion up = Quaternion.Euler(-90f, 0f, 0f);
            GameObject obj = Instantiate(healeffect, gameObject.transform.position, up);
            obj.transform.SetParent(gameObject.transform);
            UpdateHealthUI();
        }
        else
        {
            unitHealth += amount;
            Quaternion up = Quaternion.Euler(-90f, 0f, 0f);
            GameObject obj = Instantiate(healeffect, gameObject.transform.position, up);
            obj.transform.SetParent(gameObject.transform);
            UpdateHealthUI();
        }
    }
}

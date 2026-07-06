using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Harvester : MonoBehaviour
{
    public Transform assignedNode; // Assigned resource node
    public Transform supplyCenter; // Supply center for depositing resources
    public float harvestAmountPerSecond = 1f; // Harvest rate
    public UnityEngine.UI.Slider capacitySlider;
    public UnityEngine.UI.Image fillSlider;
    public float maxCapacity = 10f; // Max carrying capacity
    public float CurrentCapacity
    {
        get => _currentCapacity;
        set
        {
            _currentCapacity = Mathf.Clamp(value, 0f, maxCapacity);
            UpdateSlider();
        }
    }
    public ResourceType currentResourceType;
    public AudioClip miningSound;
    public AudioClip choppingSound;



    //public float currentCapacity = 0f; // Current resource load
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private bool isDepositing = false; // Tracks if currently in Depositing state
    private float _currentCapacity;
    private List<GameObject> allSupplyDropsList;


    private void UpdateSlider()
    {
        if (capacitySlider != null)
        {
            float normalized = CurrentCapacity / maxCapacity;
            float scaled = normalized * 10f;
            capacitySlider.value = Mathf.Round(scaled);
        }

        if (currentResourceType == ResourceType.Gold)
        {
            fillSlider.color = Color.yellow;
        }
        else if (currentResourceType == ResourceType.Wood)
        {
            fillSlider.color = Color.green;
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentResourceType = ResourceType.None;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (gameObject.CompareTag("Enemy"))
        {
            AIBehavior.Instance.RegisterWorker(this);
        }
    }

    void Update()
    {
        // Update Animator parameters
        animator.SetBool("hasAssignedNode", assignedNode != null);
        animator.SetBool("isFull", CurrentCapacity >= maxCapacity);
        animator.SetBool("isNotEmpty", CurrentCapacity > 0);

        // Check for node depletion
        if (assignedNode != null && assignedNode.GetComponent<ResourceNode>().IsDepleted)
        {
            assignedNode = null; // Clear the node
            animator.SetBool("hasAssignedNode", false); // Trigger Idle state
        }
    }

    private void OnDestroy()
    {
        if (gameObject.CompareTag("Enemy") && AIBehavior.Instance != null)
        {
            AIBehavior.Instance.UnregisterWorkerByResource(this);
            AIBehavior.Instance.UnregisterWorker(this);
        }
    }

    public void MoveTo(Transform target)
    {
        if (target == null) return;
        UnitMovement unitMovement = GetComponent<UnitMovement>();
        if (unitMovement != null)
        {
            unitMovement.MoveTo(target.position, false); // false = a command issued from the code rather than by a player
        }
    }

    public void Harvest()
    {
        if (assignedNode == null)
        {
            Cancelharvesting();
            return;
        }

        if (assignedNode != null && !assignedNode.GetComponent<ResourceNode>().IsDepleted)
        {
            ResourceNode node = assignedNode.GetComponent<ResourceNode>();
            currentResourceType = node.resourceType;
            FindNearestSupplycenter();

            // Simulate harvesting
            if (currentResourceType == ResourceType.Gold)
            {
                audioSource.clip = miningSound;
                audioSource.Play();
            }
            else if (currentResourceType == ResourceType.Wood)
            {
                audioSource.clip = choppingSound;
                audioSource.Play();
            }
            assignedNode.GetComponent<ResourceNode>().Harvest(harvestAmountPerSecond * Time.deltaTime);
            CurrentCapacity += harvestAmountPerSecond * Time.deltaTime;

            // Clamp capacity
            if (CurrentCapacity > maxCapacity)
            {
                CurrentCapacity = maxCapacity;
            }
        }
    }

    public void Cancelharvesting()
    {
        assignedNode = null;
        animator.SetBool("hasAssignedNode", false);
        animator.SetBool("atNode", false);
        animator.SetBool("isMoving", false);
        audioSource.Stop();
    }

    public void DepositResources()
    {
        if (assignedNode == null)
        {
            Cancelharvesting();
            return;
        }

        if (isDepositing) return;

        isDepositing = true;

        StartCoroutine(WaitInDepositingState());
    }

    private System.Collections.IEnumerator WaitInDepositingState()
    {
        float tempCapa = CurrentCapacity;

        while (CurrentCapacity > 0f)
        {
            CurrentCapacity -= 1 * Time.deltaTime;

            // Ensure currentCapacity doesn't go below 0
            if (CurrentCapacity < 0f)
            {
                CurrentCapacity = 0f;
            }

            yield return null; // Wait for the next frame
        }

        HandleResourceDeposit(tempCapa, currentResourceType);
        ResourceManager.Instance.UpdateUI();

        isDepositing = false;


        // Transition back to GoingToHarvest
        if (assignedNode != null)
        {
            animator.SetTrigger("doneDepositing");
        }

        currentResourceType = ResourceType.None;
    }

    private void HandleResourceDeposit(float tempCapa, ResourceType currentResourceType)
    {
        if (gameObject.CompareTag("Unit"))
        {
            switch (currentResourceType)
            {
                case ResourceType.Gold:
                    ResourceManager.Instance.IncreaseResource(ResourceType.Gold, (int)tempCapa * 3);
                    return;
                case ResourceType.Wood:
                    ResourceManager.Instance.IncreaseResource(ResourceType.Wood, (int)tempCapa * 2);
                    return;
            }
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            switch (currentResourceType)
            {
                case ResourceType.Gold:
                    AIBehavior.Instance.EnemyIncreaseResource(ResourceType.Gold, (int)tempCapa * 3);
                    return;
                case ResourceType.Wood:
                    AIBehavior.Instance.EnemyIncreaseResource(ResourceType.Wood, (int)tempCapa * 2);
                    return;
            }
        }
    }

    private void FindNearestSupplycenter()
    {
        if (assignedNode == null)
            return;

        string supplyTag = GetSupplyTagForWorker();
        if (string.IsNullOrEmpty(supplyTag))
            return;

        allSupplyDropsList = GameObject.FindGameObjectsWithTag(supplyTag).ToList();

        if (allSupplyDropsList.Count == 0)
            return;

        float closestDistance = float.MaxValue;
        Transform closestSupply = null;

        foreach (GameObject supply in allSupplyDropsList)
        {
            if (supply == null)
                continue;

            float distance = Vector3.Distance(
                assignedNode.position,
                supply.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSupply = supply.transform;
            }
        }

        if (closestSupply != null)
        {
            supplyCenter = closestSupply;
        }
    }

    private string GetSupplyTagForWorker()
    {
        if (CompareTag("Unit"))
            return "SupplyDrop";

        if (CompareTag("Enemy"))
            return "EnemySupplyDrop";

        return null;
    }

    private void OnTriggerEnter(Collider other) // Use onTriggerStay instead to check if the agent stopped moving.
    {
        if (assignedNode != null && other.transform == assignedNode)
        {
            // Reached the resource node
            animator.SetBool("atNode", true);
        }
        else if (supplyCenter != null && other.transform == supplyCenter)
        {
            // Reached the supply center
            animator.SetBool("atSupply", true);
            audioSource.Stop();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (assignedNode != null && other.transform == assignedNode)
        {
            // Left the resource node
            animator.SetBool("atNode", false);
        }
        else if (supplyCenter != null && other.transform == supplyCenter)
        {
            // Left the supply center
            animator.SetBool("atSupply", false);
        }
    }
}
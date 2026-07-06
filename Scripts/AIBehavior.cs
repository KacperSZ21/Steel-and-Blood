using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public class AIBuildingPlan
{
    public GameObject buildingObject;   // object on scene (turned off)
    public int goldCost;
    public int woodCost;
    public bool built; // only for code so it wont built twice
}

public class AIBehavior : MonoBehaviour
{
    public static AIBehavior Instance { get; private set; }

    [Header("Resources")]
    public int enemyGold;
    public int enemyWood;

    [Header("Worker Control")]
    [SerializeField] private int maxWorkers;
    [SerializeField] private EnemyBuildingScript workerProductionBuilding;
    public GameObject supplyDropForWorkers;

    [Header("Build Order")]
    [SerializeField] private List<AIBuildingPlan> buildingPlan;


    private Dictionary<ResourceType, int> workersPerResource = new();
    private int currentWorkerCount = 0;
    private float aiTickInterval = 2f;
    private float aiTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        workersPerResource[ResourceType.Gold] = 0;
        workersPerResource[ResourceType.Wood] = 0;

        InvokeRepeating(nameof(FindWorkers), 2f, 5f);
    }

    private void Update()
    {
        HandleAITick();
    }

    private void HandleAITick()
    {
        aiTimer += Time.deltaTime;

        if (aiTimer < aiTickInterval)
            return;

        aiTimer = 0f;

        TryExecuteBuildOrder();
    }

    private void TryExecuteBuildOrder()
    {
        foreach (var plan in buildingPlan)
        {
            if (plan.built)
                continue;

            if (enemyGold >= plan.goldCost && enemyWood >= plan.woodCost)
            {
                enemyGold -= plan.goldCost;
                enemyWood -= plan.woodCost;

                plan.buildingObject.SetActive(true);
                plan.built = true;

                break; // build only one on tick
            }

            break; // waiting for resoerces for first in list
        }
    }

    public void EnemyIncreaseResource(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Gold:
                enemyGold += amount;
                break;

            case ResourceType.Wood:
                enemyWood += amount;
                break;
        }
    }

    private void FindWorkers()
    {
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject unit in allUnits)
        {
            Harvester harvester = unit.GetComponent<Harvester>();

            if (harvester != null && harvester.assignedNode == null)
            {
                AssignNode(harvester);
            }
        }
    }

    public void UnregisterWorkerByResource(Harvester harvester)
    {
        if (workersPerResource.ContainsKey(harvester.currentResourceType))
        {
            workersPerResource[harvester.currentResourceType]--;
        }
    }

    private ResourceType GetLeastOccupiedResource()
    {
        ResourceType least = ResourceType.Gold;
        int min = int.MaxValue;

        foreach (var pair in workersPerResource)
        {
            if (pair.Value < min)
            {
                min = pair.Value;
                least = pair.Key;
            }
        }

        return least;
    }

    private void AssignNode(Harvester harvester)
    {
        if (supplyDropForWorkers == null)
            return;

        ResourceType chosenType = GetLeastOccupiedResource();

        GameObject[] resources = GameObject.FindGameObjectsWithTag("Environment");

        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 position = harvester.transform.position;

        foreach (GameObject res in resources)
        {
            ResourceNode node = res.GetComponent<ResourceNode>();

            if (node == null || node.resourceType != chosenType)
                continue;

            float distance = Vector3.Distance(position, res.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = res;
            }
        }

        if (closest != null)
        {
            harvester.supplyCenter = supplyDropForWorkers.transform;
            harvester.assignedNode = closest.transform;
            harvester.currentResourceType = chosenType;

            workersPerResource[chosenType]++;
        }
    }

    public void RegisterWorker(Harvester worker)
    {
        currentWorkerCount++;
        UpdateWorkerProductionState();
    }

    public void UnregisterWorker(Harvester worker)
    {
        currentWorkerCount--;
        UpdateWorkerProductionState();
    }

    private void UpdateWorkerProductionState()
    {
        if (workerProductionBuilding == null)
            return;

        if (currentWorkerCount >= maxWorkers)
        {
            workerProductionBuilding.canSpawnUnits = false;
        }
        else
        {
            workerProductionBuilding.canSpawnUnits = true;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuySlotState
{
    Hidden,
    NoResources,
    Ready,
    Training,
    ReadyToPlace
}

public class BuySlot : MonoBehaviour
{
    public BuySystem buySystem;
    public BuildSlotProgress buildSlotProgress;
    public int databaseItemID;
    public float trainingTime = 5f;
    public int populationCost;
    public Unit unitForPopulationSystem;
    public GameObject readyBG;

    private BuySlotState currentState;
    private Button button;
    private bool isBuilding;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();

        isBuilding = buySystem.IsItemaBuilding(databaseItemID);

        if (!isBuilding && readyBG != null)
        {
            readyBG.SetActive(false);
        }
    }

    private void Start()
    {
        ResourceManager.Instance.OnResourceChanged += HandleResourcesChanged;
        ResourceManager.Instance.OnBuildingChanged += HandleBuildingChanged;
        PopulationManagement.Instance.OnPopulationChanged += HandlePopulationChanged;

        RecalculateState();
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance == null) return;

        ResourceManager.Instance.OnResourceChanged -= HandleResourcesChanged;
        ResourceManager.Instance.OnBuildingChanged -= HandleBuildingChanged;
        if (PopulationManagement.Instance != null)
            PopulationManagement.Instance.OnPopulationChanged -= HandlePopulationChanged;
    }

    // =========================
    // CLICK
    // =========================
    public void ClickedOnSlot()
    {
        switch (currentState)
        {
            case BuySlotState.Ready:
                HandleReadyClick();
                break;

            case BuySlotState.ReadyToPlace:
                HandlePlaceClick();
                break;
        }
    }

    // =========================
    // STATE HANDLERS
    // =========================
    private void HandleReadyClick()
    {
        ObjectData data = buySystem.database.objectsData[databaseItemID];

        // HARD CHECK
        if (!HasRequiredResources())
            return;

        if (!buySystem.IsItemaBuilding(databaseItemID) &&
            !PopulationManagement.Instance.CanRecruitUnits(populationCost))
            return;

        ResourceManager.Instance.DecreaseResourcesBasedOnRequirement(data);
        buildSlotProgress.StartBuilding(trainingTime);

        if (buySystem.IsItemaBuilding(databaseItemID))
        {
            buySystem.StartBuildingTraining(this);
            currentState = BuySlotState.Training;
        }
        else
        {
            PopulationManagement.Instance.AddUnit(unitForPopulationSystem);

            PlayBarracksTrainingFeedback(unitForPopulationSystem);
            buySystem.StartUnitTraining(this, databaseItemID, trainingTime);

            currentState = BuySlotState.Training;
        }

        UpdateUI();
    }

    private void HandlePlaceClick()
    {
        buySystem.placementSystem.StartPlacement(databaseItemID);

        readyBG.SetActive(false);
        currentState = BuySlotState.Ready;

        RecalculateState();
    }

    // =========================
    // EXTERNAL CALLS
    // =========================
    public void SetReadyToPlace()
    {
        if (!isBuilding)
            return;

        currentState = BuySlotState.ReadyToPlace;
        readyBG?.SetActive(true);
        UpdateUI();
    }

    public void FinishTraining()
    {
        currentState = BuySlotState.Ready;
        RecalculateState();
    }

    // =========================
    // STATE CALCULATION
    // =========================
    private void RecalculateState()
    {
        bool visible = HasRequiredBuildings();
        gameObject.SetActive(visible);

        if (!visible)
        {
            currentState = BuySlotState.Hidden;
            return;
        }

        if (currentState == BuySlotState.Training ||
            currentState == BuySlotState.ReadyToPlace)
        {
            UpdateUI();
            return;
        }

        currentState = HasRequiredResources()
            ? BuySlotState.Ready
            : BuySlotState.NoResources;

        UpdateUI();
    }

    private bool HasRequiredResources()
    {
        ObjectData data = DataBaseManager.Instance.databaseSO.objectsData[databaseItemID];

        foreach (BuildRequirement req in data.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
                return false;
        }

        if (!buySystem.IsItemaBuilding(databaseItemID))
        {
            return PopulationManagement.Instance.CanRecruitUnits(populationCost);
        }

        return true;
    }

    private bool HasRequiredBuildings()
    {
        ObjectData data = DataBaseManager.Instance.databaseSO.objectsData[databaseItemID];

        foreach (BuildingType dependency in data.buildingsRequirements)
        {
            if (dependency == BuildingType.None)
                continue;

            if (!ResourceManager.Instance.allExistingBuildings.Contains(dependency))
                return false;
        }

        return true;
    }

    // =========================
    // EVENTS
    // =========================
    private void HandleResourcesChanged()
    {
        RecalculateState();
    }

    private void HandleBuildingChanged()
    {
        RecalculateState();
    }

    private void HandlePopulationChanged()
    {
        RecalculateState();
    }

    // =========================
    // UI
    // =========================
    private void UpdateUI()
    {
        button.interactable = currentState switch
        {
            BuySlotState.Ready => true,
            BuySlotState.ReadyToPlace => true,
            _ => false
        };

        if (isBuilding && readyBG != null)
        {
            readyBG.SetActive(currentState == BuySlotState.ReadyToPlace);
        }
    }

    public bool HasEnoughResources()
    {
        ObjectData data = DataBaseManager.Instance.databaseSO.objectsData[databaseItemID];

        foreach (BuildRequirement req in data.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
                return false;
        }

        return true;
    }

    public bool HasEnoughPopulation()
    {
        if (!buySystem.IsItemaBuilding(databaseItemID))
            return PopulationManagement.Instance.CanRecruitUnits(populationCost);

        return true; // buildings don't use populstion
    }

    private Constructable FindBuilding(BuildingType type)
    {
        foreach (var c in FindObjectsOfType<Constructable>())
        {
            if (c.buildingType == type)
                return c;
        }
        return null;
    }

    private void PlayTrainingFX(Constructable building)
    {
        Animator animator = building.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("OpenDoor");
        }

        ParticleSystem ps = building.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = trainingTime;

            ps.Play();
        }
    }

    private void PlayBarracksTrainingFeedback(Unit unit)
    {
        BuildingType targetBuildingType =
            unit.unitType == UnitType.Harvester
                ? BuildingType.TownHall
                : BuildingType.Barracks;

        Constructable targetBuilding = FindBuilding(targetBuildingType);

        if (targetBuilding == null)
            return;

        PlayTrainingFX(targetBuilding);
    }

    public BuySlotState CurrentState => currentState;
}
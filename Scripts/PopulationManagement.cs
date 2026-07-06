using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulationManagement : MonoBehaviour
{
    public static PopulationManagement Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI currentSupplyUI;
    [SerializeField] public TextMeshProUGUI maxSupplyUI;
    [SerializeField] private int basePopulationCap = 10;
    public event Action OnPopulationChanged;

    private int currentPopulation;
    private int populationMaxCap;

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
        populationMaxCap = basePopulationCap;

        Unit[] unitsOnMap = FindObjectsOfType<Unit>();
        foreach (var unit in unitsOnMap)
        {
            AddUnit(unit);
        }

        UpdateUI();
    }

    public void AddUnit(Unit unit)
    {
        currentPopulation += GetUnitCost(unit.unitType);
        OnPopulationChanged?.Invoke();
        UpdateUI();
    }

    public void RemoveUnit(Unit unit)
    {
        currentPopulation -= GetUnitCost(unit.unitType);
        UpdateUI();
    }

    public void AddHouse(int bonus = 10)
    {
        populationMaxCap += bonus;
        OnPopulationChanged?.Invoke();
        UpdateUI();
    }

    public void RemoveHouse(int bonus = 10)
    {
        populationMaxCap -= bonus;
        UpdateUI();
    }

    public bool CanRecruitUnits(int cost)
    {
        if (currentPopulation + cost >= 50)
        {
            return false;
        }
        return currentPopulation + cost <= populationMaxCap;
    }

    public bool MaxPopultionReached()
    {
        if (currentPopulation >= 50)
        {
            return true;
        }
        return false;
    }

    private int GetUnitCost(UnitType type)
    {
        return type switch
        {
            UnitType.LightInfantry => 1,
            UnitType.HeavyInfantry => 2,
            UnitType.DamageInfantry => 3,
            UnitType.MagicInfantry => 3,
            UnitType.Harvester => 0,
            _ => 0
        };
    }

    private void UpdateUI()
    {
        currentSupplyUI.text = currentPopulation.ToString();
        if (populationMaxCap >= 50)
        {
            maxSupplyUI.text = "50";
        }
        else
        {
            maxSupplyUI.text = populationMaxCap.ToString();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

    void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int gold;
    public int wood;

    public event Action OnResourceChanged;
    public event Action OnBuildingChanged;
    public TextMeshProUGUI goldUI;
    public TextMeshProUGUI woodUI;

    public List<BuildingType> allExistingBuildings;
    public PlacementSystem placementSystem;


    private void Start()
    {
        UpdateUI();
    }

    public void UpdateBuildingChanged(BuildingType buildingType, bool isNew, Vector3 position)
    {
        if (isNew)
        {
            allExistingBuildings.Add(buildingType);
            SoundManagerScript.Instance.PlayBuildingConstructionSound();
        }
        else
        {
            placementSystem.RemovePlacementData(position);
            allExistingBuildings.Remove(buildingType);
        }

        OnBuildingChanged?.Invoke();
    }

    public void IncreaseResource(ResourceType resource, int amoutnToIncrease)
    {
        switch (resource)
        {
            case ResourceType.Gold:
                gold += amoutnToIncrease;
                break;
            case ResourceType.Wood:
                wood += amoutnToIncrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void DecreaseResource(ResourceType resource, int amoutonToDecrease)
    {
        switch (resource)
        {
            case ResourceType.Gold:
                gold -= amoutonToDecrease;
                break;
            case ResourceType.Wood:
                wood -= amoutonToDecrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void SellBuilding(BuildingType buildingType)
    {
        SoundManagerScript.Instance.PlaySellingBuildingSound();

        var sellingPrice = 0;
        foreach (ObjectData obj in DataBaseManager.Instance.databaseSO.objectsData)
        {
            if (obj.thisBuildingType == buildingType)
            {
                if (buildingType == BuildingType.House && int.Parse(PopulationManagement.Instance.maxSupplyUI.text) >= 20)
                {
                    PopulationManagement.Instance.RemoveHouse();
                }
                foreach (BuildRequirement req in obj.resourceRequirements)
                {
                    if (req.resource == ResourceType.Gold)
                    {
                        sellingPrice = req.amount;
                    }
                }
            }
        }

        //int amountToReturn = (int)(sellingPrice * 0.75f);// reduced reimbursement of expenses

        IncreaseResource(ResourceType.Gold, sellingPrice);//amount to return instead sellingprice if type above is allowed
    }

    public int GetGold()
    {
        return gold;
    }
    public int GetWood()
    {
        return wood;
    }

    internal int GetResourceAmount(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Gold:
                return gold;
            case ResourceType.Wood:
                return wood;
            default:
                break;
        }
        return 0;
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }

    internal void ReturnResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            IncreaseResource(req.resource, req.amount);
        }
    }

    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }

    private string FormatResource(int value)
    {
        if (value >= 10000)
        {
            float thousands = value / 1000f;
            return thousands.ToString("0.#") + "k";
        }

        return value.ToString();
    }

    public void UpdateUI()
    {
        goldUI.text = FormatResource(gold);
        woodUI.text = FormatResource(wood);
    }

    public void RestartResourceManager()
    {
        allExistingBuildings.Clear();
    }
}

public enum ResourceType
{
    Gold,
    Wood,
    None
}

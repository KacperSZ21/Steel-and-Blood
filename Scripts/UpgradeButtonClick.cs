using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using System;


[Serializable]
public class UpgradeData
{
    public int id;
    public int goldCost;
    public int woodCost;
    public float buildTime;
    public Action<float> executeUpgrade;
}


public class UpgradeButtonClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BuySystem buySystem;
    public BuildSlotProgress buildSlotProgress;
    public GameObject tooltipObject;

    public TextMeshProUGUI goldCostText;
    public TextMeshProUGUI woodCostText;

    public UpgradeData upgrade;

    private Color goldOriginal;
    private Color woodOriginal;
    private Button button;

    void Awake()
    {
        button = gameObject.GetComponent<Button>();
        goldOriginal = goldCostText.color;
        woodOriginal = woodCostText.color;
    }

    void Start()
    {
        goldCostText.text = upgrade.goldCost.ToString();
        woodCostText.text = upgrade.woodCost.ToString();

        switch (upgrade.id)
        {
            case 1:
                upgrade.executeUpgrade = buySystem.StartUpgradeIncreaseDamageforLightInfatry;
                break;
            case 2:
                upgrade.executeUpgrade = buySystem.StartUpgradeIncreaseHealthforLightInfantry;
                break;
            case 3:
                upgrade.executeUpgrade = buySystem.StartUpgradeIncreaseDamageforHeavyInfatry;
                break;
            case 4:
                upgrade.executeUpgrade = buySystem.StartUpgradeIncreaseHealthforHeavyInfantry;
                break;
            case 5:
                upgrade.executeUpgrade = buySystem.StartUpgradeIncreaseDamageforMagicInfatry;
                break;
            case 6:
                upgrade.executeUpgrade = buySystem.StartUpgradeIncreaseHealthforMagicInfantry;
                break;
            case 7:
                upgrade.executeUpgrade = buySystem.StartUpgradeHealSpellforMagicInfantry;
                break;
            case 8:
                upgrade.executeUpgrade = buySystem.StartUpgradeAOESpellforMagicInfantry;
                break;
            default:
                Debug.LogError("Invalid upgrade ID: " + upgrade.id);
                button.interactable = false; // Disable button if ID is invalid
                return;
        }
    }

    public void OnClick()
    {
        if (!CanAfford())
            return;

        SpendResources();
        buildSlotProgress.StartBuilding(upgrade.buildTime);
        upgrade.executeUpgrade?.Invoke(upgrade.buildTime);
    }

    bool CanAfford()
    {
        return ResourceManager.Instance.GetGold() >= upgrade.goldCost &&
               ResourceManager.Instance.GetWood() >= upgrade.woodCost;
    }

    void SpendResources()
    {
        ResourceManager.Instance.DecreaseResource(ResourceType.Gold, upgrade.goldCost);
        ResourceManager.Instance.DecreaseResource(ResourceType.Wood, upgrade.woodCost);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipObject.SetActive(true);
        RefreshTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.SetActive(false);
        ResetColors();
    }

    void RefreshTooltip()
    {
        goldCostText.color = ResourceManager.Instance.GetGold() >= upgrade.goldCost
            ? goldOriginal
            : Color.red;

        woodCostText.color = ResourceManager.Instance.GetWood() >= upgrade.woodCost
            ? woodOriginal
            : Color.red;
    }

    void ResetColors()
    {
        goldCostText.color = goldOriginal;
        woodCostText.color = woodOriginal;
    }
}
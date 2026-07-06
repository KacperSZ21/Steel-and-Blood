using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BuySystem : MonoBehaviour
{

    [Header("Panels")]
    public GameObject buildingsPanel;
    public GameObject unitsPanel;

    [Header("UI")]
    public Button buildingsButton;
    public Button unitsButton;
    public Sprite pressedButtonSprite;
    public Sprite normalButtonSprite;

    [Header("Systems")]
    public PlacementSystem placementSystem;

    [SerializeField]
    internal ObjectsDatabseSO database;

    [SerializeField]
    public List<string> activeUpgradesList = new();

    #region |--- Unit Training ---|

    public bool IsItemaBuilding(int id)
    {
        //return true if this is building
        return database.GetObjectByID(id).thisBuildingType != BuildingType.None;
    }

    internal IEnumerator StartUnitTrainingCoroutine(BuySlot buySlot, int databaseItemID, float traniTime)
    {
        ObjectData unitToTrain = database.GetObjectByID(databaseItemID);
        if (unitToTrain.thisUnitType == UnitType.None)
        {
            Debug.LogError("Forgot to assign a unit type");
        }
        yield return new WaitForSeconds(traniTime);

        if (unitToTrain.Name == "Worker")
        {
            InstantiateUnit(unitToTrain.Name, true);
        }
        else
        {
            InstantiateUnit(unitToTrain.Name);
        }
        buySlot.FinishTraining();
    }

    private void InstantiateUnit(string name, bool worker = false)
    {
        Constructable[] allConstructables = FindObjectsOfType<Constructable>();

        Constructable barracks = null;

        foreach (var b in allConstructables)
        {
            if (worker)
            {
                if (b.buildingType == BuildingType.TownHall)
                {
                    barracks = b;
                    break;
                }
            }
            else
            {
                if (b.buildingType == BuildingType.Barracks)
                {
                    barracks = b;
                    break;
                }
            }
        }


        if (barracks == null)
        {
            //Should not happen
            Debug.LogError("No barracks found");
        }


        GameObject unitPrefab = Resources.Load<GameObject>(name);

        if (unitPrefab == null)
        {
            Debug.LogError("Could not find unit prefab. Maybe name is wrong");
        }

        Vector3 entranceposition = barracks.transform.position + new Vector3(3f, 0, 0.5f);
        Vector3 randomoffset = new(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
        Vector3 finalSpawnPosition = entranceposition + randomoffset;

        barracks.GetComponent<Animator>().SetTrigger("CloseDoor");

        GameObject newUnit = Instantiate(unitPrefab, finalSpawnPosition, Quaternion.identity);
        if (worker)
        {
            newUnit.GetComponent<Harvester>().supplyCenter = barracks.transform.Find("SupplyDrop");
            return;
        }

        foreach (string upgradeName in activeUpgradesList)
        {
            if (upgradeName == "DamageIncrease")
            {
                if (newUnit.GetComponent<Attackcontroller>() != null)
                {
                    if (newUnit.GetComponent<Unit>().unitType == UnitType.LightInfantry)
                    {
                        newUnit.GetComponent<Attackcontroller>().IncreaseDamage(10);
                    }
                }
            }
            else if (upgradeName == "HealthIncrease")
            {
                if (newUnit.GetComponent<Unit>() != null)
                {
                    if (newUnit.GetComponent<Unit>().unitType == UnitType.LightInfantry)
                    {
                        newUnit.GetComponent<Unit>().IncreaseHealth(20);
                    }
                }
            }
            else if (upgradeName == "HeavyDamageIncrease")
            {
                if (newUnit.GetComponent<Attackcontroller>() != null)
                {
                    if (newUnit.GetComponent<Unit>().unitType == UnitType.HeavyInfantry || newUnit.GetComponent<Unit>().unitType == UnitType.DamageInfantry)
                    {
                        newUnit.GetComponent<Attackcontroller>().IncreaseDamage(20);
                    }
                }
            }
            else if (upgradeName == "HeavyHealthIncrease")
            {
                if (newUnit.GetComponent<Unit>() != null)
                {
                    if (newUnit.GetComponent<Unit>().unitType == UnitType.HeavyInfantry || newUnit.GetComponent<Unit>().unitType == UnitType.DamageInfantry)
                    {
                        newUnit.GetComponent<Unit>().IncreaseHealth(40);
                    }
                }
            }
            else if (upgradeName == "MagicHealthIncrease")
            {
                if (newUnit.GetComponent<Unit>() != null)
                {
                    if (newUnit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
                    {
                        newUnit.GetComponent<Unit>().IncreaseHealth(20);
                    }
                }
            }
            else if (upgradeName == "MagicDamageIncrease")
            {
                if (newUnit.GetComponent<Attackcontroller>() != null)
                {
                    if (newUnit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
                    {
                        newUnit.GetComponent<Attackcontroller>().IncreaseDamage(30);
                    }
                }
            }
            else if (upgradeName == "AddHealSpell")
            {
                if (newUnit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
                {
                    newUnit.GetComponent<HealSpell>().enabled = true;
                }
            }
            else if (upgradeName == "AddAOESpell")
            {
                if (newUnit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
                {
                    newUnit.GetComponent<SpecialMagicAttack>().enabled = true;
                }
            }
        }

        barracks.GetComponent<Animator>().SetTrigger("CloseDoor");
    }

    public void StartUnitTraining(BuySlot buySlot, int databaseItemID, float traniTime) // Bridge Method
    {
        StartCoroutine(StartUnitTrainingCoroutine(buySlot, databaseItemID, traniTime));
    }

    public void StartBuildingTraining(BuySlot slot) // Bridge Method
    {
        StartCoroutine(StartBuildingTrainingCoroutine(slot));
    }

    private IEnumerator StartBuildingTrainingCoroutine(BuySlot slot)
    {
        yield return new WaitForSeconds(slot.trainingTime);

        slot.SetReadyToPlace();
    }


    #endregion





    #region |--- UI Related ---|
    void Start()
    {
        unitsButton.onClick.AddListener(UnitsCategorySelected);
        buildingsButton.onClick.AddListener(BuildingsCategorySelected);

        buildingsPanel.SetActive(true);
        unitsPanel.SetActive(false);
        if (buildingsButton.GetComponent<Button>().interactable == false)
        {
            buildingsButton.GetComponent<Image>().sprite = normalButtonSprite;
        }
        else
        {
            buildingsButton.GetComponent<Image>().sprite = pressedButtonSprite;
        }
    }

    private void BuildingsCategorySelected()
    {
        unitsPanel.SetActive(false);
        buildingsPanel.SetActive(true);
        buildingsButton.GetComponent<Image>().sprite = pressedButtonSprite;
        unitsButton.GetComponent<Image>().sprite = normalButtonSprite;
    }

    private void UnitsCategorySelected()
    {
        buildingsPanel.SetActive(false);
        unitsPanel.SetActive(true);
        unitsButton.GetComponent<Image>().sprite = pressedButtonSprite;
        buildingsButton.GetComponent<Image>().sprite = normalButtonSprite;
    }

    #endregion





    #region |-- Unit Upgrades --|

    private IEnumerator UpgradeIncreaseDamageforLightInfatry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Attackcontroller>() != null)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.LightInfantry)
                {
                    unit.GetComponent<Attackcontroller>().IncreaseDamage(10);
                }
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("DamageIncrease");
    }

    public void StartUpgradeIncreaseDamageforLightInfatry(float upgradeTime)
    {
        StartCoroutine(UpgradeIncreaseDamageforLightInfatry(upgradeTime));
    }

    private IEnumerator UpgradeIncreaseHealthforLightInfantry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Unit>() != null)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.LightInfantry)
                {
                    unit.GetComponent<Unit>().IncreaseHealth(20);
                }
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("HealthIncrease");
    }
    public void StartUpgradeIncreaseHealthforLightInfantry(float upgradeTime)
    {
        StartCoroutine(UpgradeIncreaseHealthforLightInfantry(upgradeTime));
    }

    private IEnumerator UpgradeIncreaseDamageforHeavyInfatry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Attackcontroller>() != null)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.HeavyInfantry || unit.GetComponent<Unit>().unitType == UnitType.DamageInfantry)
                {
                    unit.GetComponent<Attackcontroller>().IncreaseDamage(20);
                }
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("HeavyDamageIncrease");
    }

    public void StartUpgradeIncreaseDamageforHeavyInfatry(float upgradeTime)
    {
        StartCoroutine(UpgradeIncreaseDamageforHeavyInfatry(upgradeTime));
    }


    private IEnumerator UpgradeIncreaseHealthforHeavyInfantry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Unit>() != null)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.HeavyInfantry || unit.GetComponent<Unit>().unitType == UnitType.DamageInfantry)
                {
                    unit.GetComponent<Unit>().IncreaseHealth(40);
                }
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("HeavyHealthIncrease");
    }

    public void StartUpgradeIncreaseHealthforHeavyInfantry(float upgradeTime)
    {
        StartCoroutine(UpgradeIncreaseHealthforHeavyInfantry(upgradeTime));
    }

    private IEnumerator UpgradeIncreaseHealthforMagicInfantry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Unit>() != null)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
                {
                    unit.GetComponent<Unit>().IncreaseHealth(20);
                }
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("MagicHealthIncrease");
    }

    public void StartUpgradeIncreaseHealthforMagicInfantry(float upgradeTime)
    {
        StartCoroutine(UpgradeIncreaseHealthforMagicInfantry(upgradeTime));
    }

    private IEnumerator UpgradeIncreaseDamageforMagicInfatry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Attackcontroller>() != null)
            {
                if (unit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
                {
                    unit.GetComponent<Attackcontroller>().IncreaseDamage(30);
                }
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("MagicDamageIncrease");
    }

    public void StartUpgradeIncreaseDamageforMagicInfatry(float upgradeTime)
    {
        StartCoroutine(UpgradeIncreaseDamageforMagicInfatry(upgradeTime));
    }

    private IEnumerator UpgradeHealSpellforMagicInfantry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
            {
                unit.GetComponent<HealSpell>().enabled = true;
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("AddHealSpell");
    }

    public void StartUpgradeHealSpellforMagicInfantry(float upgradeTime)
    {
        StartCoroutine(UpgradeHealSpellforMagicInfantry(upgradeTime));
    }

    private IEnumerator UpgradeAOESpellforMagicInfantry(float upgradeTime)
    {
        yield return new WaitForSeconds(upgradeTime);

        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.GetComponent<Unit>().unitType == UnitType.MagicInfantry)
            {
                unit.GetComponent<SpecialMagicAttack>().enabled = true;
            }
            else
            {
                continue;
            }
        }

        activeUpgradesList.Add("AddAOESpell");
    }

    public void StartUpgradeAOESpellforMagicInfantry(float upgradeTime)
    {
        StartCoroutine(UpgradeAOESpellforMagicInfantry(upgradeTime));
    }

    #endregion

    public void RestartBuySystem()
    {
        activeUpgradesList.Clear();
    }
}

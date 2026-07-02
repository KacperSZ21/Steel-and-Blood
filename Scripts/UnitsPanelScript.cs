using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnitsPanelScript : MonoBehaviour
{
    public UnitSelectionManager unitSelectionManager;
    [Header("Light Infantry Panel")]
    public GameObject lightInfantryPanel;
    public GameObject lightDMGValue;
    public GameObject lightHealthValue;
    public GameObject lightAmount;

    [Header("Heavy Infantry Panel")]
    public GameObject heavyInfantryPanel;
    public GameObject heavyDMGValue;
    public GameObject heavyHealthValue;
    public GameObject heavyAmount;

    [Header("Damage Infantry Panel")]
    public GameObject damageInfantryPanel;
    public GameObject damageDMGValue;
    public GameObject damageHealthValue;
    public GameObject damageAmount;

    [Header("Magic Infantry Panel")]
    public GameObject magicInfantryPanel;
    public GameObject magicDMGValue;
    public GameObject magicHealthValue;
    public GameObject magicAmount;

    void Start()
    {
        TurnOff();
    }

    // Update is called once per frame
    void Update()
    {
        if (unitSelectionManager.unitsSelected.Count > 0)
        {
            for (int i = 0; i < unitSelectionManager.unitsSelected.Count; i++)
            {
                var unit = unitSelectionManager.unitsSelected[i];
                if (unit == null)
                {
                    unitSelectionManager.unitsSelected.RemoveAt(i);
                    continue;
                }

                switch (unit.GetComponent<Unit>().unitType)
                {
                    case UnitType.LightInfantry:
                        lightInfantryPanel.SetActive(true);
                        lightDMGValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Attackcontroller>().unitDamage.ToString();
                        lightHealthValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Unit>().unitMaxHealth.ToString();
                        lightAmount.GetComponent<TextMeshProUGUI>().text = GiveUnitsAmount(UnitType.LightInfantry).ToString();
                        break;
                    case UnitType.HeavyInfantry:
                        heavyInfantryPanel.SetActive(true);
                        heavyDMGValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Attackcontroller>().unitDamage.ToString();
                        heavyHealthValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Unit>().unitMaxHealth.ToString();
                        heavyAmount.GetComponent<TextMeshProUGUI>().text = GiveUnitsAmount(UnitType.HeavyInfantry).ToString();
                        break;
                    case UnitType.DamageInfantry:
                        damageInfantryPanel.SetActive(true);
                        damageDMGValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Attackcontroller>().unitDamage.ToString();
                        damageHealthValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Unit>().unitMaxHealth.ToString();
                        damageAmount.GetComponent<TextMeshProUGUI>().text = GiveUnitsAmount(UnitType.DamageInfantry).ToString();
                        break;
                    case UnitType.MagicInfantry:
                        magicInfantryPanel.SetActive(true);
                        magicDMGValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Attackcontroller>().unitDamage.ToString();
                        magicHealthValue.GetComponent<TextMeshProUGUI>().text = unit.GetComponent<Unit>().unitMaxHealth.ToString();
                        magicAmount.GetComponent<TextMeshProUGUI>().text = GiveUnitsAmount(UnitType.MagicInfantry).ToString();
                        break;
                    default:
                        return;
                }
            }
        }
        else
        {
            TurnOff();
        }
    }

    void TurnOff()
    {
        lightInfantryPanel.SetActive(false);
        heavyInfantryPanel.SetActive(false);
        damageInfantryPanel.SetActive(false);
        magicInfantryPanel.SetActive(false);
    }

    private int GiveUnitsAmount(UnitType unitType)
    {
        List<GameObject> unitsWithOneType = new();

        for (int i = unitSelectionManager.unitsSelected.Count - 1; i >= 0; i--)
        {
            var unit = unitSelectionManager.unitsSelected[i];

            if (unit == null)
            {
                unitSelectionManager.unitsSelected.RemoveAt(i);
                continue;
            }

            if (unit.GetComponent<Unit>().unitType == unitType)
            {
                unitsWithOneType.Add(unit);
            }
        }

        return unitsWithOneType.Count;
    }
}
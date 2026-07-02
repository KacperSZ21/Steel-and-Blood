using System.Collections;
using UnityEngine;

public class HealSpell : MonoBehaviour
{
    public float radius = 5f;
    public float spellCooldown = 2f;
    public float healAmount = 10f;

    private void Start()
    {
        StartCoroutine(CastHealSpell());
    }

    private IEnumerator CastHealSpell()
    {
        while (true)
        {
            Unit unitToHeal = FindMostInjuredUnitInRange();

            if (unitToHeal != null)
            {
                unitToHeal.HealUnit(healAmount);
            }

            yield return new WaitForSeconds(spellCooldown);
        }
    }

    private Unit FindMostInjuredUnitInRange()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius
        );

        Unit bestTarget = null;
        float lowestHealthPercent = 1f;

        foreach (var hit in hits)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit == null) continue;

            if (unit.Gethealth() >= unit.unitMaxHealth)
                continue;

            float hpPercent = unit.Gethealth() / unit.unitMaxHealth;

            if (hpPercent < lowestHealthPercent)
            {
                lowestHealthPercent = hpPercent;
                bestTarget = unit;
            }
        }

        return bestTarget;
    }
}

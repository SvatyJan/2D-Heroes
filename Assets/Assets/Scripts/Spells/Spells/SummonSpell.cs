using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Summon Unit")]
public class SummonSpell : Spell
{
    [Header("Summon")]
    public GameObject unitPrefab;
    public float castRange = 6f;

    [Header("Costs")]
    public int soulCost = 0;
    public ManaCostEntry[] manaCosts;

    public override void Cast(Player caster, Vector2 worldPosition)
    {
        var souls = caster.GetComponent<SoulResource>();
        var mana = caster.GetComponent<HeroMana>();

        if (souls == null || mana == null)
            return;

        // --- Souls ---
        if (soulCost > 0 && !souls.CanSpend(soulCost))
        {
            Debug.Log("Not enough souls.");
            return;
        }

        // --- Mana ---
        if (!ManaUtility.CanPayCosts(mana, manaCosts))
        {
            Debug.Log("Not enough mana.");
            return;
        }

        // --- Range ---
        float distance = Vector2.Distance(
            caster.transform.position,
            worldPosition
        );

        if (distance > castRange)
        {
            Debug.Log("Target too far.");
            return;
        }

        // PAY
        if (soulCost > 0)
            souls.SpendSouls(soulCost);

        ManaUtility.PayCosts(mana, manaCosts);

        // SPAWN
        GameObject unit = Instantiate(
            unitPrefab,
            worldPosition,
            Quaternion.identity
        );

        var unitBehavior = unit.GetComponent<UnitBehavior>();
        if (unitBehavior != null)
            unitBehavior.SetOwner(caster);

        Debug.Log("Unit summoned.");
    }
}
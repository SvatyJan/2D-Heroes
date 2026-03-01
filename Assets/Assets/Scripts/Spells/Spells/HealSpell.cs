using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Heal")]
public class HealSpell : Spell
{
    public float totalHeal = 100f;
    public float duration = 3f;
    public float castRange = 6f;

    public override void Cast(Player caster, Vector2 worldPosition)
    {
        var mana = caster.GetComponent<HeroMana>();
        var souls = caster.GetComponent<SoulResource>();

        if (mana == null)
            return;

        // Souls
        if (soulCost > 0)
        {
            if (souls == null || !souls.CanSpend(soulCost))
                return;
        }

        // Mana
        if (!ManaUtility.CanPayCosts(mana, manaCosts))
            return;

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit == null)
            return;

        UnitBehavior unit = hit.GetComponent<UnitBehavior>();
        if (unit == null)
            return;

        if (unit.Owner != caster)
            return;

        float distance = Vector2.Distance(
            caster.transform.position,
            unit.transform.position
        );

        if (distance > castRange)
            return;

        // PAY
        if (soulCost > 0)
            souls.SpendSouls(soulCost);

        ManaUtility.PayCosts(mana, manaCosts);

        HealOverTime hot = unit.GetComponent<HealOverTime>();
        if (hot == null)
            hot = unit.gameObject.AddComponent<HealOverTime>();

        hot.Apply(totalHeal, duration);
    }
}
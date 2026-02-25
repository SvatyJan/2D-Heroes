using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Holy Light")]
public class HolyLightSpell : Spell
{
    public float manaCost = 30f;
    public float radius = 5f;
    public float healAmount = 100f;
    public float castRange = 8f;

    public override void Cast(Player caster, Vector2 worldPosition)
    {
        var mana = caster.GetComponent<HeroMana>();
        if (mana == null)
            return;

        if (ManaUtility.GetTotalMana(mana) < manaCost)
        {
            Debug.Log("Not enough mana.");
            return;
        }

        float distance = Vector2.Distance(
            caster.transform.position,
            worldPosition
        );

        if (distance > castRange)
        {
            Debug.Log("Target too far.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, radius);

        int healedCount = 0;

        foreach (var hit in hits)
        {
            UnitBehavior unit = hit.GetComponent<UnitBehavior>();
            if (unit == null)
                continue;

            if (unit.Owner != caster)
                continue;

            Health health = unit.GetComponent<Health>();
            if (health == null)
                continue;

            health.Heal(healAmount);
            healedCount++;
        }

        ManaUtility.PayLightPlusAny(mana, 0f, manaCost);

        Debug.Log($"Holy Light healed {healedCount} units.");
    }
}
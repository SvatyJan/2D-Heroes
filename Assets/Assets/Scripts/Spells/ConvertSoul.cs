using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Convert Soul")]
public class ConvertSoul : Spell
{
    public float castRange = 5f;
    public float manaCost = 10f;

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

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit == null)
            return;

        SoulOrb orb = hit.GetComponent<SoulOrb>();
        if (orb == null)
            return;

        if (orb.Owner == caster)
        {
            Debug.Log("Soul already owned.");
            return;
        }

        float distance = Vector2.Distance(
            caster.transform.position,
            orb.transform.position
        );

        if (distance > castRange)
        {
            Debug.Log("Soul too far.");
            return;
        }

        ManaUtility.PayLightPlusAny(mana, 0f, manaCost);

        orb.SetOwner(caster);

        Debug.Log("Soul converted.");
    }
}
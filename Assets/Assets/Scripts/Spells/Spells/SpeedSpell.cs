using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Speed")]
public class SpeedSpell : Spell
{
    public float manaCost = 20f;
    public float duration = 5f;
    public float multiplier = 1.5f;
    public float castRange = 6f;

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

        UnitBehavior unit = hit.GetComponent<UnitBehavior>();
        if (unit == null)
            return;

        // jen přátelské jednotky
        if (unit.Owner != caster)
        {
            Debug.Log("Target is not friendly.");
            return;
        }

        float distance = Vector2.Distance(
            caster.transform.position,
            unit.transform.position
        );

        if (distance > castRange)
        {
            Debug.Log("Target too far.");
            return;
        }

        ManaUtility.PayLightPlusAny(mana, 0f, manaCost);

        SpeedBuff buff = unit.GetComponent<SpeedBuff>();
        if (buff == null)
            buff = unit.gameObject.AddComponent<SpeedBuff>();

        buff.Apply(multiplier, duration);

        Debug.Log("Speed applied.");
    }
}
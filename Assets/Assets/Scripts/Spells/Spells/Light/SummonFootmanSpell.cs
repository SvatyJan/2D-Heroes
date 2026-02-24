using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Summon Footman")]
public class SummonFootmanSpell : Spell
{
    public int soulCost = 1;
    public float requiredLight = 20f;
    public float requiredAny = 20f;

    public GameObject footmanPrefab;
    public float castRange = 6f;

    public override void Cast(Player caster, Vector2 worldPosition)
    {
        var souls = caster.GetComponent<SoulResource>();
        var mana = caster.GetComponent<HeroMana>();

        if (!souls.CanSpend(soulCost))
        {
            Debug.Log("Not enough souls.");
            return;
        }

        var manaCheck = ManaUtility.CanPayLightPlusAny(
            mana,
            requiredLight,
            requiredAny
        );

        if (manaCheck != ManaCostResult.Success)
        {
            Debug.Log(manaCheck.ToString());
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

        // PAY
        souls.SpendSouls(soulCost);
        ManaUtility.PayLightPlusAny(mana, requiredLight, requiredAny);

        // SPAWN
        GameObject unit = Instantiate(
            footmanPrefab,
            worldPosition,
            Quaternion.identity
        );

        var unitBehavior = unit.GetComponent<UnitBehavior>();
        if (unitBehavior != null)
            unitBehavior.SetOwner(caster);

        Debug.Log("Footman summoned.");
    }
}
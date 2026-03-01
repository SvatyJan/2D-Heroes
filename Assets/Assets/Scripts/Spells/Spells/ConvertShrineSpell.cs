using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Convert Shrine")]
public class ConvertShrineSpell : Spell
{
    public float castRange = 5f;
    public float detectionRadius = 1.2f;

    public override void Cast(Player caster, Vector2 worldPosition)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius);

        Shrine targetShrine = null;

        foreach (var hit in hits)
        {
            Shrine shrine = hit.GetComponent<Shrine>();
            if (shrine != null)
            {
                targetShrine = shrine;
                break;
            }
        }

        if (targetShrine == null)
            return;

        float distance = Vector2.Distance(
            caster.transform.position,
            targetShrine.transform.position
        );

        if (distance > castRange)
            return;

        targetShrine.SetOwner(caster);
    }
}
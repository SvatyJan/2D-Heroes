using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Convert Shrine")]
public class ConvertShrineSpell : Spell
{
    public float castRange = 5f;

    public override void Cast(Player caster, Vector2 worldPosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
            return;

        Shrine shrine = hit.GetComponent<Shrine>();
        if (shrine == null)
            return;

        float distance = Vector2.Distance(
            caster.transform.position,
            shrine.transform.position
        );

        if (distance > castRange)
            return;

        shrine.SetOwner(caster);
        Debug.Log("Shrine converted.");
    }
}
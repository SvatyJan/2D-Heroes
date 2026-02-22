using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Spell selectedSpell;

    public Spell SelectedSpell => selectedSpell;

    public void SelectSpell(Spell spell)
    {
        selectedSpell = spell;
    }

    public void ClearSpell()
    {
        selectedSpell = null;
    }

    public void CastCurrentSpell(Player caster, Vector2 worldPosition)
    {
        if (selectedSpell == null)
            return;

        selectedSpell.Cast(caster, worldPosition);
    }
}
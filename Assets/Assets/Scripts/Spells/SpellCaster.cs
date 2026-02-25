using UnityEngine;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Spell selectedSpell;

    private Dictionary<Spell, float> cooldownTimers =
        new Dictionary<Spell, float>();

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

        if (!CanCast(selectedSpell))
        {
            Debug.Log("Spell on cooldown.");
            return;
        }

        selectedSpell.Cast(caster, worldPosition);

        cooldownTimers[selectedSpell] = Time.time;
    }

    private bool CanCast(Spell spell)
    {
        if (!cooldownTimers.ContainsKey(spell))
            return true;

        float lastCastTime = cooldownTimers[spell];
        return Time.time >= lastCastTime + spell.cooldown;
    }
}
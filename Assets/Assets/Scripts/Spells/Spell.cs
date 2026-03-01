using UnityEngine;

public abstract class Spell : ScriptableObject
{
    [Header("General")]
    public string spellName;
    public float cooldown = 0f;

    [Header("Costs")]
    public int soulCost = 0;
    public ManaCostEntry[] manaCosts;

    public abstract void Cast(Player caster, Vector2 worldPosition);
}
using UnityEngine;

public abstract class Spell : ScriptableObject
{
    public string spellName;
    public abstract void Cast(Player caster, Vector2 worldPosition);
}
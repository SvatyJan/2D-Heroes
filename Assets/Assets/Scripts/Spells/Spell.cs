using UnityEngine;

public abstract class Spell : ScriptableObject
{
    public string spellName;
    public float cooldown = 0f;

    public abstract void Cast(Player caster, Vector2 worldPosition);
}
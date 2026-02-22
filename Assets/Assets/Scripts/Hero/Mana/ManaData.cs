using UnityEngine;

[System.Serializable]
public class ManaData
{
    public ManaType type;

    [Header("Base Values (Hero Identity)")]
    public float baseMaxMana = 100f;
    public float baseRegen = 0f;

    [Header("Runtime Bonuses (Shrines, Effects)")]
    public float bonusMaxMana = 0f;
    public float bonusRegen = 0f;

    [Header("Runtime State")]
    public float currentMana = 0f;

    public float MaxMana => baseMaxMana + bonusMaxMana;
    public float Regen => baseRegen + bonusRegen;
}
[System.Serializable]
public class ManaData
{
    public ManaType type;

    public float baseMaxMana = 100f;
    public float bonusMaxMana = 0f;

    public float currentMana = 0f;

    public float baseRegen = 0f;

    public float MaxMana => baseMaxMana + bonusMaxMana;
}
using System.Collections.Generic;
using UnityEngine;

public class HeroMana : MonoBehaviour
{
    [System.Serializable]
    public class ManaData
    {
        public ManaType type;

        [Header("Base (Hero Identity)")]
        public float baseMaxMana = 100f;
        public float baseRegen = 0f;

        [Header("Runtime Bonuses (Shrines etc.)")]
        public float bonusMaxMana = 0f;
        public float bonusRegen = 0f;

        [Header("Runtime State")]
        public float currentMana = 0f;

        public float MaxMana => baseMaxMana + bonusMaxMana;
        public float Regen => baseRegen + bonusRegen;
    }

    [SerializeField]
    private List<ManaData> manaTypes = new List<ManaData>();

    private Dictionary<ManaType, ManaData> manaLookup;

    private void Awake()
    {
        manaLookup = new Dictionary<ManaType, ManaData>();

        foreach (var mana in manaTypes)
        {
            if (!manaLookup.ContainsKey(mana.type))
                manaLookup.Add(mana.type, mana);

            mana.currentMana = Mathf.Clamp(mana.currentMana, 0f, mana.MaxMana);
        }
    }

    private void Update()
    {
        Regenerate();
    }

    private void Regenerate()
    {
        foreach (var mana in manaTypes)
        {
            if (mana.MaxMana <= 0f)
                continue;

            if (mana.Regen <= 0f)
                continue;

            mana.currentMana += mana.Regen * Time.deltaTime;
            mana.currentMana = Mathf.Min(mana.currentMana, mana.MaxMana);
        }
    }

    // --------------------------
    // Shrine Bonuses
    // --------------------------

    public void AddBonus(ManaType type, float maxMana, float regen)
    {
        var mana = GetOrCreateMana(type);

        mana.bonusMaxMana += maxMana;
        mana.bonusRegen += regen;
    }

    public void RemoveBonus(ManaType type, float maxMana, float regen)
    {
        if (!manaLookup.ContainsKey(type))
            return;

        var mana = manaLookup[type];

        mana.bonusMaxMana -= maxMana;
        mana.bonusRegen -= regen;

        mana.bonusMaxMana = Mathf.Max(0f, mana.bonusMaxMana);
        mana.bonusRegen = Mathf.Max(0f, mana.bonusRegen);

        mana.currentMana = Mathf.Min(mana.currentMana, mana.MaxMana);
    }

    // --------------------------
    // Mana Spending
    // --------------------------

    public bool SpendMana(ManaType type, float amount)
    {
        if (!manaLookup.ContainsKey(type))
            return false;

        var mana = manaLookup[type];

        if (mana.currentMana < amount)
            return false;

        mana.currentMana -= amount;
        return true;
    }

    public void AddMana(ManaType type, float amount)
    {
        var mana = GetOrCreateMana(type);

        mana.currentMana += amount;
        mana.currentMana = Mathf.Min(mana.currentMana, mana.MaxMana);
    }

    // --------------------------
    // Getters
    // --------------------------

    public float GetMana(ManaType type)
    {
        if (!manaLookup.ContainsKey(type))
            return 0f;

        return manaLookup[type].currentMana;
    }

    public float GetMaxMana(ManaType type)
    {
        if (!manaLookup.ContainsKey(type))
            return 0f;

        return manaLookup[type].MaxMana;
    }

    public float GetManaPercent(ManaType type)
    {
        if (!manaLookup.ContainsKey(type))
            return 0f;

        var mana = manaLookup[type];

        if (mana.MaxMana <= 0f)
            return 0f;

        return mana.currentMana / mana.MaxMana;
    }

    // --------------------------
    // Utility
    // --------------------------

    public ManaData GetOrCreateMana(ManaType type)
    {
        if (manaLookup.ContainsKey(type))
            return manaLookup[type];

        ManaData newMana = new ManaData
        {
            type = type,
            baseMaxMana = 0f,
            baseRegen = 0f,
            bonusMaxMana = 0f,
            bonusRegen = 0f,
            currentMana = 0f
        };

        manaTypes.Add(newMana);
        manaLookup.Add(type, newMana);

        return newMana;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class HeroMana : MonoBehaviour
{
    [System.Serializable]
    public class ManaData
    {
        public ManaType type;

        [Header("Base Values")]
        public float baseMaxMana = 100f;
        public float baseRegen = 0f;

        [Header("Runtime")]
        public float bonusMaxMana = 0f;
        public float currentMana = 0f;

        public float MaxMana => baseMaxMana + bonusMaxMana;
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

            // clamp při startu
            mana.currentMana = Mathf.Clamp(mana.currentMana, 0f, mana.MaxMana);
        }
    }

    private void Update()
    {
        RegenerateBase();
    }

    private void RegenerateBase()
    {
        foreach (var mana in manaTypes)
        {
            if (mana.MaxMana <= 0f)
                continue;

            if (mana.baseRegen <= 0f)
                continue;

            mana.currentMana += mana.baseRegen * Time.deltaTime;
            mana.currentMana = Mathf.Min(mana.currentMana, mana.MaxMana);
        }
    }

    public void AddMaxMana(ManaType type, float amount)
    {
        if (!manaLookup.ContainsKey(type))
            return;

        var mana = manaLookup[type];
        mana.bonusMaxMana += amount;
    }

    public void RemoveMaxMana(ManaType type, float amount)
    {
        if (!manaLookup.ContainsKey(type))
            return;

        var mana = manaLookup[type];
        mana.bonusMaxMana -= amount;
        mana.bonusMaxMana = Mathf.Max(0f, mana.bonusMaxMana);

        // clamp current mana pokud jsme přes nový max
        mana.currentMana = Mathf.Min(mana.currentMana, mana.MaxMana);
    }

    public void AddMana(ManaType type, float amount)
    {
        if (!manaLookup.ContainsKey(type))
            return;

        var mana = manaLookup[type];

        mana.currentMana += amount;
        mana.currentMana = Mathf.Min(mana.currentMana, mana.MaxMana);
    }

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
}
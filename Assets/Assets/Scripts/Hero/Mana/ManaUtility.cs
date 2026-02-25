using UnityEngine;

public static class ManaUtility
{
    public static float GetTotalMana(HeroMana mana)
    {
        float total = 0f;

        foreach (ManaType type in System.Enum.GetValues(typeof(ManaType)))
        {
            total += mana.GetMana(type);
        }

        return total;
    }

    public static ManaCostResult CanPayLightPlusAny(
        HeroMana mana,
        float requiredLight,
        float requiredAny)
    {
        if (mana.GetMana(ManaType.Light) < requiredLight)
            return ManaCostResult.NotEnoughLight;

        float totalRequired = requiredLight + requiredAny;

        if (GetTotalMana(mana) < totalRequired)
            return ManaCostResult.NotEnoughTotal;

        return ManaCostResult.Success;
    }

    public static void PayLightPlusAny(
        HeroMana mana,
        float requiredLight,
        float requiredAny)
    {
        // zaplať povinný Light
        mana.SpendMana(ManaType.Light, requiredLight);

        float remaining = requiredAny;

        foreach (ManaType type in System.Enum.GetValues(typeof(ManaType)))
        {
            float available = mana.GetMana(type);

            if (available <= 0f)
                continue;

            float toSpend = Mathf.Min(available, remaining);
            mana.SpendMana(type, toSpend);

            remaining -= toSpend;

            if (remaining <= 0f)
                break;
        }
    }

    public static bool CanPayExactCosts(HeroMana mana, ManaCostEntry[] costs)
    {
        foreach (var cost in costs)
        {
            if (mana.GetMana(cost.type) < cost.amount)
                return false;
        }

        return true;
    }

    public static void PayExactCosts(
        HeroMana mana,
        ManaCostEntry[] costs)
    {
        foreach (var cost in costs)
        {
            mana.SpendMana(cost.type, cost.amount);
        }
    }

    public static bool CanPayCosts(HeroMana mana, ManaCostEntry[] costs)
    {
        float totalMana = GetTotalMana(mana);

        foreach (var cost in costs)
        {
            if (cost.isAnyType)
            {
                if (totalMana < cost.amount)
                    return false;

                totalMana -= cost.amount;
            }
            else
            {
                if (mana.GetMana(cost.type) < cost.amount)
                    return false;

                totalMana -= cost.amount;
            }
        }

        return true;
    }

    public static void PayCosts(HeroMana mana, ManaCostEntry[] costs)
    {
        foreach (var cost in costs)
        {
            if (cost.isAnyType)
            {
                SpendAny(mana, cost.amount);
            }
            else
            {
                mana.SpendMana(cost.type, cost.amount);
            }
        }
    }

    private static void SpendAny(HeroMana mana, float amount)
    {
        float remaining = amount;

        foreach (ManaType type in System.Enum.GetValues(typeof(ManaType)))
        {
            float available = mana.GetMana(type);
            if (available <= 0f)
                continue;

            float toSpend = Mathf.Min(available, remaining);
            mana.SpendMana(type, toSpend);

            remaining -= toSpend;

            if (remaining <= 0f)
                break;
        }
    }
}
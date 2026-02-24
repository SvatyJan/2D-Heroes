using UnityEngine;

public class SoulResource : MonoBehaviour
{
    [SerializeField] private int maxSouls = 99;
    [SerializeField] private int currentSouls = 0;

    public int CurrentSouls => currentSouls;
    public int MaxSouls => maxSouls;

    public void AddSouls(int amount)
    {
        if (amount <= 0) return;

        currentSouls += amount;
        currentSouls = Mathf.Clamp(currentSouls, 0, maxSouls);
    }

    public bool CanSpend(int amount)
    {
        return currentSouls >= amount;
    }

    public bool SpendSouls(int amount)
    {
        if (!CanSpend(amount))
            return false;

        currentSouls -= amount;
        return true;
    }

    public void SetSouls(int value)
    {
        currentSouls = Mathf.Clamp(value, 0, maxSouls);
    }
}
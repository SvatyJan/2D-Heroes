using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Soul Drop")]
    [SerializeField] private int soulValue = 1;
    [SerializeField] private GameObject soulOrbPrefab;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0f;

    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1f, value);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();

        SpawnSouls();

        Destroy(gameObject);
    }

    private void SpawnSouls()
    {
        if (soulOrbPrefab == null)
            return;

        if (soulValue <= 0)
            return;

        UnitBehavior unitBehavior = GetComponent<UnitBehavior>();
        if (unitBehavior == null)
            return;

        for (int i = 0; i < soulValue; i++)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.5f;

            GameObject orb = Instantiate(
                soulOrbPrefab,
                (Vector2)transform.position + offset,
                Quaternion.identity
            );

            SoulOrb soul = orb.GetComponent<SoulOrb>();
            if (soul != null)
            {
                soul.Initialize(unitBehavior.Owner, 1);
            }
        }
    }
}
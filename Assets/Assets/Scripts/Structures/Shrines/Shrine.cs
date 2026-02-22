using UnityEngine;

public class Shrine : MonoBehaviour, IOwned
{
    [Header("Ownership")]
    [SerializeField] private Player owner;
    public Player Owner => owner;

    [Header("Mana")]
    [SerializeField] private ManaType manaType;
    [SerializeField] private float bonusMaxMana = 100f;

    [Header("Regen")]
    [SerializeField] private float nearRegen = 10f;
    [SerializeField] private float midRegen = 1f;
    [SerializeField] private float farRegen = 0.1f;

    [SerializeField] private float nearDistance = 5f;
    [SerializeField] private float midDistance = 1000f;

    private bool bonusApplied = false;

    private void Update()
    {
        if (owner == null) return;

        HandleRegen();
    }

    public void SetOwner(Player newOwner)
    {
        // odebrat bonus starému ownerovi
        if (owner != null && bonusApplied)
        {
            owner.Mana.RemoveMaxMana(manaType, bonusMaxMana);
            bonusApplied = false;
            owner.UnregisterShrine(this);
        }

        owner = newOwner;

        if (owner != null)
        {
            owner.RegisterShrine(this);
            owner.Mana.AddMaxMana(manaType, bonusMaxMana);
            bonusApplied = true;
        }

        UpdateVisual();
    }

    private void HandleRegen()
    {
        if (owner.HeroTransform == null) return;

        float distance = Vector2.Distance(
            transform.position,
            owner.HeroTransform.position
        );

        float regenRate;

        if (distance <= nearDistance)
            regenRate = nearRegen;
        else if (distance <= midDistance)
            regenRate = midRegen;
        else
            regenRate = farRegen;

        owner.Mana.AddMana(manaType, regenRate * Time.deltaTime);
    }

    private void UpdateVisual()
    {
        if (owner == null) return;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = owner.PlayerColor;
    }
}
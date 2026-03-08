using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IOwned
{
    public Player Owner => this;

    [Header("Identity")]
    [SerializeField] private int playerId;
    [SerializeField] private Color playerColor = Color.blue;

    [Header("References")]
    [SerializeField] private Transform heroTransform;

    [Header("Systems")]
    [SerializeField] private HeroMana heroMana;

    [SerializeField] private List<UnitBehavior> units = new();
    [SerializeField] private List<Shrine> shrines = new();

    public int PlayerId => playerId;
    public Color PlayerColor => playerColor;
    public Transform HeroTransform => heroTransform;
    public HeroMana Mana => heroMana;

    public void SetOwner(Player newOwner)
    {
        // Player je sám sobě owner, takže to nedává smysl měnit.
        // Metoda existuje jen kvůli IOwned interface.
        Debug.LogWarning("Player.SetOwner() called. Player cannot change owner.");
    }

    void Awake()
    {
        if(heroTransform == null)
            heroTransform = GetComponent<Transform>();

        if (heroMana == null)
            heroMana = GetComponent<HeroMana>();
    }

    public void RegisterUnit(UnitBehavior unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);
    }

    public void UnregisterUnit(UnitBehavior unit)
    {
        units.Remove(unit);
    }

    public void RegisterShrine(Shrine shrine)
    {
        if (!shrines.Contains(shrine))
            shrines.Add(shrine);
    }

    public void UnregisterShrine(Shrine shrine)
    {
        shrines.Remove(shrine);
    }

    public List<UnitBehavior> GetUnits()
    {
        return units;
    }

    public static void UpdateVisualOwnerColor(GameObject gameObject, Player owner)
    {
        var sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = owner.PlayerColor;
    }
}
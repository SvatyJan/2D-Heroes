using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
}
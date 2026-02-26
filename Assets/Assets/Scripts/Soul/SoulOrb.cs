using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SoulOrb : MonoBehaviour, IOwned
{
    [SerializeField] private Player owner;
    [SerializeField] private int soulValue = 1;

    public Player Owner => owner;
    public int Value => soulValue;

    public void Initialize(Player newOwner, int value)
    {
        owner = newOwner;
        soulValue = value;
        UpdateVisual();
    }

    public void SetOwner(Player newOwner)
    {
        owner = newOwner;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // TODO: barva podle ownera
    }
}
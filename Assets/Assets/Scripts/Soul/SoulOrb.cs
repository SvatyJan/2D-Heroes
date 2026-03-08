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
    }

    public void SetOwner(Player newOwner)
    {
        owner = newOwner;
        Player.UpdateVisualOwnerColor(this.gameObject, owner);
    }
}
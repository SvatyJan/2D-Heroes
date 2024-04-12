using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private float mana = 100f;
    [SerializeField] private float maxMana = 100f;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxSpeed = 10f;

    [SerializeField] private float damage = 10f;

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public float Mana
    {
        get { return mana; }
        set { mana = value; }
    }

    public float MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float MaxSpeed
    {
        get { return maxSpeed; }
        set { maxSpeed = value; }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
}

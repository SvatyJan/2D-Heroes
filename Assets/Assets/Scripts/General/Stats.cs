using UnityEngine;

public class Stats : MonoBehaviour
{

    private static float speed = 100f;
    private float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    private static float maxSpeed = 100f;
    private float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
        set
        {
            maxSpeed = value;
        }
    }

    private static float damage = 10f;
    private float Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }

    private static float health = 100f;
    private float Health {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    private static float maxHealth = 100f;
    private float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    private static float mana = 100f;
    private float Mana
    {
        get
        {
            return mana;
        }
        set
        {
            mana = value;
        }
    }

    private static float maxMana = 100f;
    private float MaxMana
    {
        get
        {
            return maxMana;
        }
        set
        {
            maxMana = value;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public float health = 100f;

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public float TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            //knockback a hit effect
        }

        if (health <= 0)
        {
            health = 0;
            Die();
        }
        return health;
    }

    public float GetHealth()
    {
        return health;
    }

    public void Die()
    {
        Debug.Log(this.gameObject + " has died.");
        Destroy(this.gameObject, 5);
    }
}

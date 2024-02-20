using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{

    Damage damageComponent;
    float damage;
    [SerializeField] public string colliderObjectTag = "Enemy Unit"; // bude jich víc, takže pole

    void Start()
    {

        damageComponent = GetComponentInParent<Damage>();
        damage = damageComponent.damage;
        //damageComponent = GetComponent<Damage>();
        //damage = GetComponentInParent<Damage>().damage;

        Debug.Log("ColliderScript Start " + damage);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        /*Debug.Log("Collision!");
        damageComponent = GetComponentInParent<Damage>();
        damage = damageComponent.damage;

        if (collision.gameObject.CompareTag(colliderObjectTag))
        {
            Debug.Log(collision.gameObject.name + " hit!");
            damageComponent.DealDamage(damage, collision.gameObject);
        }*/
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        damageComponent = GetComponentInParent<Damage>();
        damage = damageComponent.damage;

        if (collision.gameObject.CompareTag(colliderObjectTag))
        {            
            damageComponent.DealDamage(damage, collision.gameObject);
        }
    }
}

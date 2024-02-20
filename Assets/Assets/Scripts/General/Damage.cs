using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public float damage = 10f;

    void Start()
    {

    }
    void Update()
    {

    }

    public void DealDamage(float damage, GameObject target)
    {
        Debug.Log(target.name + " hit for " + damage + " damage.");
        target.GetComponent<Health>().TakeDamage(damage);
    }
}

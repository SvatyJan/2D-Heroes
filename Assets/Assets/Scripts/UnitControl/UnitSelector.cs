using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    /**
     * Metoda pro select jednotky pøi kolizi se selectorem.
     * */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject unit = collision.gameObject;
        if (unit.tag == "Player Unit")
        {
            GameObject hero = this.transform.parent.gameObject;
            unit.GetComponent<UnitBehavior>().AddToGroup(hero);
        }        
    }
}

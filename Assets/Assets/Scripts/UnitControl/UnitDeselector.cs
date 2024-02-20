using UnityEngine;

public class UnitDeselector : MonoBehaviour
{
    /**
     * Metoda pro select jednotky p�i kolizi se selectorem.
     * */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject unit = collision.gameObject;
        if (unit.tag == "Player Unit")
        {
            GameObject hero = this.transform.parent.gameObject;
            unit.GetComponent<UnitBehavior>().RemoveFromGroup(hero);
        }
        else if(unit.tag == "Player Flag")
        {
            this.gameObject.transform.parent.GetComponent<PlayerController>().removeFlag(unit);
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SoulCollector : MonoBehaviour
{
    private SoulResource soulResource;
    private Player player;

    private void Awake()
    {
        soulResource = GetComponent<SoulResource>();
        player = GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoulOrb orb = collision.GetComponent<SoulOrb>();
        if (orb == null)
            return;

        // Jen pokud orb patří tomuto hráči
        if (orb.Owner != player)
            return;

        if (soulResource != null)
        {
            soulResource.AddSouls(orb.Value);
            Destroy(orb.gameObject);
        }
    }
}
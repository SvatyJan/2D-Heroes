using UnityEngine;

public class HeartStructure : MonoBehaviour, IOwned
{
    [SerializeField] private Player owner;

    public Player Owner => owner;

    public void SetOwner(Player newOwner)
    {
        owner = newOwner;
    }

    private void Awake()
    {
        Health health = GetComponent<Health>();
        Player.UpdateVisualOwnerColor(this.gameObject, owner);

        if (health != null)
            health.OnDeath += HandleDestroyed;
    }

    private void HandleDestroyed()
    {
        Debug.Log(owner.name + " heart destroyed!");

        //GameManager.Instance.PlayerDefeated(owner);
    }
}
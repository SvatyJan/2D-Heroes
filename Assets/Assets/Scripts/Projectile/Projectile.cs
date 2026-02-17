using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;

    private Damage ownerDamage;
    private float spawnTime;
    private Rigidbody2D rb;

    public void Init(Vector2 dir, Damage damageSource)
    {
        ownerDamage = damageSource;
        spawnTime = Time.time;

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir.normalized * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    private void Update()
    {
        if (Time.time > spawnTime + lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ownerDamage == null) return;

        if (collision.CompareTag("Enemy Unit"))
        {
            ownerDamage.DealDamage(ownerDamage.damage, collision.gameObject);
            Destroy(gameObject);
        }
    }
}

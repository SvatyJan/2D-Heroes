using UnityEngine;

public class RangedAttack : AttackBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public override void ExecuteAttack(GameObject target)
    {
        if (target == null) return;

        Vector2 direction = (target.transform.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile p = projectile.GetComponent<Projectile>();
        if (p != null)
            p.Init(direction, GetComponent<Damage>());
    }
}

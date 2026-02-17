using UnityEngine;

public class MeleeAttack : AttackBehaviour
{
    public float attackRadius = 2f;
    private Damage damageComponent;

    private void Awake()
    {
        damageComponent = GetComponent<Damage>();
    }

    public override void ExecuteAttack(GameObject target)
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.transform.position);
        if (dist > attackRadius) return;

        damageComponent.DealDamage(damageComponent.damage, target);
    }
}

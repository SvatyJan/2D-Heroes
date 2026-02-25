using UnityEngine;
using System.Collections;

public class HealOverTime : MonoBehaviour
{
    private Coroutine activeRoutine;

    public void Apply(float totalHeal, float duration)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(HealRoutine(totalHeal, duration));
    }

    private IEnumerator HealRoutine(float totalHeal, float duration)
    {
        Health health = GetComponent<Health>();
        if (health == null)
            yield break;

        float elapsed = 0f;
        float healPerSecond = totalHeal / duration;

        while (elapsed < duration)
        {
            float delta = Time.deltaTime;
            health.Heal(healPerSecond * delta);

            elapsed += delta;
            yield return null;
        }

        activeRoutine = null;
    }
}
using UnityEngine;
using System.Collections;

public class SpeedBuff : MonoBehaviour
{
    private Coroutine activeRoutine;

    public void Apply(float multiplier, float duration)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(ApplyRoutine(multiplier, duration));
    }

    private IEnumerator ApplyRoutine(float multiplier, float duration)
    {
        UnitBehavior unit = GetComponent<UnitBehavior>();
        if (unit == null)
            yield break;

        unit.ModifyMoveSpeed(multiplier);

        yield return new WaitForSeconds(duration);

        unit.ModifyMoveSpeed(1f / multiplier);

        activeRoutine = null;
    }
}
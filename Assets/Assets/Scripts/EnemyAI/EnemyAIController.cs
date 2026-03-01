using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public enum StrategicState
    {
        Farm,
        CaptureShrine,
        AttackPlayer,
        AttackHeart
    }

    public enum AIProfile
    {
        Aggressive,
        Defensive,
        Balanced
    }

    [Header("Hero Reference")]
    public Transform heroTransform;
    public Player selfPlayer;

    [Header("Enemy References")]
    public Player enemyPlayer;
    public Transform enemyHeart;

    [Header("World")]
    public List<Shrine> shrines = new();

    [Header("AI Settings")]
    public AIProfile profile = AIProfile.Balanced;
    public float decisionInterval = 0.5f;
    public float moveSpeed = 3f;
    public float neutralSearchRadius = 20f;

    private StrategicState currentState;
    private float decisionTimer;

    private void Update()
    {
        if (heroTransform == null || selfPlayer == null)
            return;

        decisionTimer += Time.deltaTime;

        if (decisionTimer >= decisionInterval)
        {
            decisionTimer = 0f;
            EvaluateDecision();
        }

        ExecuteState();
        ControlUnits();
    }

    #region DECISION

    void EvaluateDecision()
    {
        float myPower = CalculatePower(selfPlayer);
        float enemyPower = CalculatePower(enemyPlayer);

        float powerRatio = myPower / Mathf.Max(enemyPower, 1f);

        float farmScore = FindClosestNeutralUnit() != null ? 5f : 0f;
        float shrineScore = CountNeutralShrines() * 3f;
        float attackScore = 0f;

        if (powerRatio > 1.2f)
            attackScore = 15f;
        else if (powerRatio > 1.0f)
            attackScore = 8f;

        if (profile == AIProfile.Aggressive)
            attackScore *= 1.5f;

        if (profile == AIProfile.Defensive)
            farmScore *= 1.5f;

        float max = Mathf.Max(farmScore, shrineScore, attackScore);

        if (max == attackScore)
            currentState = StrategicState.AttackPlayer;
        else if (max == shrineScore)
            currentState = StrategicState.CaptureShrine;
        else
            currentState = StrategicState.Farm;
    }

    #endregion

    #region EXECUTION

    void ExecuteState()
    {
        switch (currentState)
        {
            case StrategicState.Farm:
                UnitBehavior neutralUnit = FindClosestNeutralUnit();
                if (neutralUnit != null)
                    MoveHeroTo(neutralUnit.transform.position);
                break;

            case StrategicState.CaptureShrine:
                Shrine shrine = FindClosestNeutralShrine();
                if (shrine != null)
                    MoveHeroTo(shrine.transform.position);
                break;

            case StrategicState.AttackPlayer:
                if (enemyPlayer != null)
                    MoveHeroTo(enemyPlayer.transform.position);
                break;

            case StrategicState.AttackHeart:
                if (enemyHeart != null)
                    MoveHeroTo(enemyHeart.position);
                break;
        }
    }

    #endregion

    #region UNIT CONTROL

    void ControlUnits()
    {
        List<UnitBehavior> units = selfPlayer.GetUnits();
        if (units == null || units.Count == 0) return;

        foreach (var unit in units)
        {
            if (unit == null) continue;

            unit.setFollowTarget(heroTransform.gameObject);
            unit.setStance(UnitBehavior.Stance.DEFENSIVE);
        }
    }

    #endregion

    #region HELPERS

    float CalculatePower(Player player)
    {
        if (player == null) return 0f;

        float power = 0f;

        Health heroHealth = player.GetComponent<Health>();
        if (heroHealth != null)
            power += heroHealth.CurrentHealth;

        List<UnitBehavior> units = player.GetUnits();
        if (units != null)
        {
            foreach (var unit in units)
            {
                if (unit == null) continue;

                Health h = unit.GetComponent<Health>();
                if (h != null)
                    power += h.CurrentHealth;
            }
        }

        return power;
    }

    int CountNeutralShrines()
    {
        int count = 0;

        foreach (var shrine in shrines)
        {
            if (shrine == null) continue;
            if (shrine.Owner == null)
                count++;
        }

        return count;
    }

    Shrine FindClosestNeutralShrine()
    {
        Shrine closest = null;
        float minDist = float.MaxValue;

        foreach (var shrine in shrines)
        {
            if (shrine == null) continue;
            if (shrine.Owner != null) continue;

            float dist = Vector2.Distance(heroTransform.position, shrine.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = shrine;
            }
        }

        return closest;
    }

    UnitBehavior FindClosestNeutralUnit()
    {
        UnitBehavior[] allUnits = GameObject.FindObjectsOfType<UnitBehavior>();

        float minDist = float.MaxValue;
        UnitBehavior closest = null;

        foreach (var unit in allUnits)
        {
            if (unit == null) continue;
            if (unit.Owner != null) continue;

            float dist = Vector2.Distance(heroTransform.position, unit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = unit;
            }
        }

        return closest;
    }

    void MoveHeroTo(Vector2 position)
    {
        heroTransform.position = Vector2.MoveTowards(
            heroTransform.position,
            position,
            moveSpeed * Time.deltaTime);
    }

    #endregion
}
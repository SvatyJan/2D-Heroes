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

    [Header("Hero")]
    public Transform heroTransform;
    public Player selfPlayer;

    [Header("Enemy")]
    public Player enemyPlayer;
    public Transform enemyHeart;

    [Header("World")]
    public List<Shrine> shrines = new();

    [Header("Spell System")]
    public SpellCaster spellCaster;
    public HeroMana heroMana;
    [SerializeField] private List<Spell> availableSpells = new();

    [Header("AI Settings")]
    public AIProfile profile = AIProfile.Balanced;
    public float decisionInterval = 0.5f;
    public float moveSpeed = 3f;

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
            EvaluateSpells();
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

        float shrineScore = CountNeutralShrines() * 5f;
        float attackScore = 0f;

        if (powerRatio > 1.2f)
            attackScore = 10f;

        float max = Mathf.Max(shrineScore, attackScore);

        if (max == shrineScore)
            currentState = StrategicState.CaptureShrine;
        else
            currentState = StrategicState.AttackPlayer;

        Debug.Log($"[AI] State -> {currentState}");
    }

    #endregion

    #region EXECUTION

    void ExecuteState()
    {
        switch (currentState)
        {
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

    #region SPELL AI

    void EvaluateSpells()
    {
        if (spellCaster == null || heroMana == null)
            return;

        Shrine neutralShrine = FindClosestNeutralShrine();

        if (neutralShrine == null)
            return;

        foreach (var spell in availableSpells)
        {
            if (spell == null)
                continue;

            // Hledáme convert shrine spell (podle názvu)
            if (!spell.name.ToLower().Contains("convert"))
                continue;

            if (!heroMana.CanAfford(spell))
            {
                Debug.Log("[AI] Not enough mana for Convert Shrine.");
                continue;
            }

            Debug.Log("[AI] Casting Convert Shrine on: " + neutralShrine.name);

            spellCaster.SelectSpell(spell);
            spellCaster.CastCurrentSpell(selfPlayer, neutralShrine.transform.position);
            spellCaster.ClearSpell();

            break; // castíme jen jeden spell za tick
        }
    }

    #endregion

    #region UNIT CONTROL

    void ControlUnits()
    {
        List<UnitBehavior> units = selfPlayer.GetUnits();
        if (units == null || units.Count == 0)
            return;

        foreach (var unit in units)
        {
            if (unit == null)
                continue;

            unit.setFollowTarget(heroTransform.gameObject);
            unit.setStance(UnitBehavior.Stance.DEFENSIVE);
        }
    }

    #endregion

    #region HELPERS

    float CalculatePower(Player player)
    {
        if (player == null)
            return 0f;

        float power = 0f;

        Health heroHealth = player.GetComponent<Health>();
        if (heroHealth != null)
            power += heroHealth.CurrentHealth;

        List<UnitBehavior> units = player.GetUnits();

        if (units != null)
        {
            foreach (var unit in units)
            {
                if (unit == null)
                    continue;

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
            if (shrine == null)
                continue;

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
            if (shrine == null)
                continue;

            if (shrine.Owner != null)
                continue;

            float dist = Vector2.Distance(
                heroTransform.position,
                shrine.transform.position
            );

            if (dist < minDist)
            {
                minDist = dist;
                closest = shrine;
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
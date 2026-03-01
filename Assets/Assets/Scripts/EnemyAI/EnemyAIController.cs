using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public enum StrategicState
    {
        Idle,
        MovingToShrine,
        CapturingShrine,
        AttackingPlayer,
        AttackingHeart
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
    public float decisionInterval = 0.5f;
    public float moveSpeed = 3f;
    public float shrineCaptureRadius = 5f;

    private StrategicState currentState = StrategicState.Idle;
    private float decisionTimer;

    private Shrine currentTargetShrine;

    private void Update()
    {
        if (heroTransform == null || selfPlayer == null)
            return;

        decisionTimer += Time.deltaTime;

        if (decisionTimer >= decisionInterval)
        {
            decisionTimer = 0f;
            EvaluateHighLevelDecision();
        }

        ExecuteState();
        ControlUnits();
    }

    #region HIGH LEVEL DECISION

    void EvaluateHighLevelDecision()
    {
        if (currentTargetShrine != null &&
            currentTargetShrine.Owner == null)
        {
            currentState = StrategicState.MovingToShrine;
            return;
        }

        currentTargetShrine = FindClosestNeutralShrine();

        if (currentTargetShrine != null)
        {
            Debug.Log("[AI] Targeting shrine: " + currentTargetShrine.name);
            currentState = StrategicState.MovingToShrine;
            return;
        }

        if (enemyPlayer != null)
        {
            currentState = StrategicState.AttackingPlayer;
            return;
        }

        currentState = StrategicState.Idle;
    }

    #endregion

    #region STATE EXECUTION

    void ExecuteState()
    {
        switch (currentState)
        {
            case StrategicState.MovingToShrine:
                HandleMoveToShrine();
                break;

            case StrategicState.CapturingShrine:
                HandleCaptureShrine();
                break;

            case StrategicState.AttackingPlayer:
                if (enemyPlayer != null)
                    MoveHeroTo(enemyPlayer.transform.position);
                break;

            case StrategicState.AttackingHeart:
                if (enemyHeart != null)
                    MoveHeroTo(enemyHeart.position);
                break;
        }
    }

    void HandleMoveToShrine()
    {
        if (currentTargetShrine == null)
        {
            currentState = StrategicState.Idle;
            return;
        }

        if (currentTargetShrine.Owner != null)
        {
            Debug.Log("[AI] Shrine already taken.");
            currentTargetShrine = null;
            currentState = StrategicState.Idle;
            return;
        }

        float distance = Vector2.Distance(
            heroTransform.position,
            currentTargetShrine.transform.position
        );

        if (distance > shrineCaptureRadius)
        {
            MoveHeroTo(currentTargetShrine.transform.position);
        }
        else
        {
            Debug.Log("[AI] In range of shrine. Switching to capture.");
            currentState = StrategicState.CapturingShrine;
        }
    }

    void HandleCaptureShrine()
    {
        if (currentTargetShrine == null)
        {
            currentState = StrategicState.Idle;
            return;
        }

        if (currentTargetShrine.Owner != null)
        {
            Debug.Log("[AI] Shrine captured.");
            currentTargetShrine = null;
            currentState = StrategicState.Idle;
            return;
        }

        Spell convertSpell = GetConvertShrineSpell();

        if (convertSpell == null)
        {
            Debug.Log("[AI] No Convert Shrine spell available.");
            return;
        }

        if (!heroMana.CanAfford(convertSpell))
        {
            Debug.Log("[AI] Not enough mana.");
            return;
        }

        if (spellCaster.IsOnCooldown(convertSpell))
        {
            Debug.Log("[AI] Convert spell on cooldown.");
            return;
        }

        Debug.Log("[AI] Casting Convert Shrine on " + currentTargetShrine.name);

        spellCaster.SelectSpell(convertSpell);
        spellCaster.CastCurrentSpell(selfPlayer, currentTargetShrine.transform.position);
        spellCaster.ClearSpell();
    }

    #endregion

    #region UNIT CONTROL

    void ControlUnits()
    {
        List<UnitBehavior> units = selfPlayer.GetUnits();
        if (units == null) return;

        foreach (var unit in units)
        {
            if (unit == null) continue;

            unit.setFollowTarget(heroTransform.gameObject);
            unit.setStance(UnitBehavior.Stance.DEFENSIVE);
        }
    }

    #endregion

    #region HELPERS

    Shrine FindClosestNeutralShrine()
    {
        Shrine closest = null;
        float minDist = float.MaxValue;

        foreach (var shrine in shrines)
        {
            if (shrine == null) continue;
            if (shrine.Owner != null) continue;

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

    Spell GetConvertShrineSpell()
    {
        foreach (var spell in availableSpells)
        {
            if (spell != null &&
                spell.name.ToLower().Contains("convert"))
            {
                return spell;
            }
        }

        return null;
    }

    void MoveHeroTo(Vector2 position)
    {
        heroTransform.position = Vector2.MoveTowards(
            heroTransform.position,
            position,
            moveSpeed * Time.deltaTime
        );
    }

    #endregion
}
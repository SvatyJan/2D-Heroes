using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public enum StrategicState
    {
        Idle,
        MovingToShrine,
        CapturingShrine,
        CollectingSouls,
        SummoningArmy,
        Defending,
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
    [SerializeField] private float soulSearchRadius = 12f;
    [SerializeField] private int lowSoulThreshold = 3;
    [SerializeField] private float summonDistance = 2f;
    [SerializeField] private float defendPositionTolerance = 1.25f;
    [SerializeField] private float attackPowerMultiplier = 1.1f;
    [SerializeField] private float shrinePowerWeight = 1.5f;
    [SerializeField] private float soulPowerWeight = 0.15f;

    [Header("Unit Coordination")]
    [SerializeField] private ObjectFormationController heroFormationController;
    [SerializeField] private UnitBehavior.Formation defaultUnitFormation = UnitBehavior.Formation.CIRCLE;
    [SerializeField] private UnitBehavior.Stance defendUnitStance = UnitBehavior.Stance.DEFENSIVE;
    [SerializeField] private UnitBehavior.Stance attackUnitStance = UnitBehavior.Stance.AGRESSIVE;
    [SerializeField] private float defaultCircleSpacing = 3f;

    [Header("Combat")]
    [SerializeField] private float heroAttackRange = 1.5f;
    [SerializeField] private float startTimeBtwAttack = 1f;
    [SerializeField] private float attackRecoveryTimeout = 1.5f;

    private StrategicState currentState = StrategicState.Idle;
    private float decisionTimer;
    private float timeBtwAttack;
    private float attackStartedAt = -999f;

    private Shrine currentTargetShrine;
    private SoulOrb currentTargetSoul;
    private Damage heroDamage;
    [SerializeField] private Animator animator;
    private GameObject pendingAttackTarget;
    private bool attackInProgress;

    private void Awake()
    {
        if (heroTransform == null)
            heroTransform = transform;

        heroDamage = GetComponent<Damage>();

        if (animator == null)
            animator = selfPlayer.GetComponent<Animator>();

        if (heroFormationController == null && selfPlayer != null)
            heroFormationController = selfPlayer.GetComponent<ObjectFormationController>();

        if (heroFormationController != null)
            heroFormationController.SetSpacingCircle(defaultCircleSpacing);
    }

    private void Update()
    {
        if (heroTransform == null || selfPlayer == null)
            return;

        if (timeBtwAttack > 0f)
            timeBtwAttack -= Time.deltaTime;

        RecoverStuckAttack();

        decisionTimer += Time.deltaTime;

        if (decisionTimer >= decisionInterval)
        {
            decisionTimer = 0f;
            EvaluateHighLevelDecision();
        }

        ExecuteState();
        ControlUnits();
    }

    void EvaluateHighLevelDecision()
    {
        currentTargetShrine = FindClosestShrineToContest();
        currentTargetSoul = FindClosestCollectibleSoul();

        if (ShouldCaptureShrine(currentTargetShrine))
        {
            float distanceToShrine = Vector2.Distance(
                heroTransform.position,
                currentTargetShrine.transform.position
            );

            currentState = distanceToShrine <= shrineCaptureRadius
                ? StrategicState.CapturingShrine
                : StrategicState.MovingToShrine;
            return;
        }

        if (ShouldCollectSoul(currentTargetSoul))
        {
            currentState = StrategicState.CollectingSouls;
            return;
        }

        if (CanCastSpell(GetSummonSpell()))
        {
            currentState = StrategicState.SummoningArmy;
            return;
        }

        if (IsStrongerThanEnemy())
        {
            currentState = enemyPlayer != null
                ? StrategicState.AttackingPlayer
                : StrategicState.AttackingHeart;
            return;
        }

        currentState = StrategicState.Defending;
    }

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

            case StrategicState.CollectingSouls:
                HandleCollectSouls();
                break;

            case StrategicState.SummoningArmy:
                HandleSummoning();
                break;

            case StrategicState.Defending:
                HandleDefending();
                break;

            case StrategicState.AttackingPlayer:
                HandleAttackPlayer();
                break;

            case StrategicState.AttackingHeart:
                HandleAttackHeart();
                break;
        }
    }

    void HandleMoveToShrine()
    {
        if (!ShouldCaptureShrine(currentTargetShrine))
        {
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
            currentState = StrategicState.CapturingShrine;
        }
    }

    void HandleCaptureShrine()
    {
        if (!ShouldCaptureShrine(currentTargetShrine))
        {
            currentState = StrategicState.Idle;
            return;
        }

        Spell convertSpell = GetConvertShrineSpell();
        if (!CanCastSpell(convertSpell))
            return;

        spellCaster.SelectSpell(convertSpell);
        spellCaster.CastCurrentSpell(selfPlayer, currentTargetShrine.transform.position);
        spellCaster.ClearSpell();
    }

    void HandleCollectSouls()
    {
        if (currentTargetSoul == null)
        {
            currentState = StrategicState.Idle;
            return;
        }

        MoveHeroTo(currentTargetSoul.transform.position);
    }

    void HandleSummoning()
    {
        SummonSpell summonSpell = GetSummonSpell();
        if (!CanCastSpell(summonSpell))
        {
            currentState = StrategicState.Idle;
            return;
        }

        Vector2 summonTarget = GetSummonPosition(summonSpell.castRange);

        spellCaster.SelectSpell(summonSpell);
        spellCaster.CastCurrentSpell(selfPlayer, summonTarget);
        spellCaster.ClearSpell();
    }

    void HandleDefending()
    {
        Shrine ownedShrine = FindClosestOwnedShrine(selfPlayer);
        if (ownedShrine == null)
            return;

        float distance = Vector2.Distance(
            heroTransform.position,
            ownedShrine.transform.position
        );

        if (distance > defendPositionTolerance)
            MoveHeroTo(ownedShrine.transform.position);
    }

    void HandleAttackPlayer()
    {
        if (enemyPlayer == null)
        {
            currentState = StrategicState.Idle;
            return;
        }

        HandleHeroCombat(enemyPlayer.gameObject);
    }

    void HandleAttackHeart()
    {
        if (enemyHeart == null)
        {
            currentState = StrategicState.Idle;
            return;
        }

        HandleHeroCombat(enemyHeart.gameObject);
    }

    void HandleHeroCombat(GameObject target)
    {
        if (target == null)
        {
            currentState = StrategicState.Idle;
            return;
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth == null || !targetHealth.IsAlive)
        {
            currentState = StrategicState.Idle;
            pendingAttackTarget = null;
            return;
        }

        LookAtTarget(target.transform.position);

        if (attackInProgress)
            return;

        float distance = Vector2.Distance(
            heroTransform.position,
            target.transform.position
        );

        if (distance > heroAttackRange)
        {
            MoveHeroTo(target.transform.position);
            return;
        }

        StartHeroMeleeAttack(target);
    }

    void StartHeroMeleeAttack(GameObject target)
    {
        if (animator == null)
            return;

        if (timeBtwAttack > 0f)
            return;

        pendingAttackTarget = target;
        attackInProgress = true;
        attackStartedAt = Time.time;
        animator.SetBool("Attacking", true);
        timeBtwAttack = startTimeBtwAttack;
    }

    public void OnAttackHitFrame()
    {
        if (heroDamage == null || pendingAttackTarget == null)
            return;

        Health targetHealth = pendingAttackTarget.GetComponent<Health>();
        if (targetHealth == null || !targetHealth.IsAlive)
            return;

        IOwned owned = pendingAttackTarget.GetComponent<IOwned>();
        if (owned != null && owned.Owner == selfPlayer)
            return;

        float distance = Vector2.Distance(
            heroTransform.position,
            pendingAttackTarget.transform.position
        );

        if (distance > heroAttackRange)
            return;

        heroDamage.DealDamage(heroDamage.damage, pendingAttackTarget);
    }

    public void OnAttackFinished()
    {
        ResetAttackState();
    }

    void RecoverStuckAttack()
    {
        if (!attackInProgress)
            return;

        if (animator != null && !animator.GetBool("Attacking"))
        {
            ResetAttackState();
            return;
        }

        if (Time.time >= attackStartedAt + attackRecoveryTimeout)
            ResetAttackState();
    }

    void ResetAttackState()
    {
        attackInProgress = false;
        pendingAttackTarget = null;
        attackStartedAt = -999f;

        if (animator != null)
            animator.SetBool("Attacking", false);
    }

    void ControlUnits()
    {
        List<UnitBehavior> units = selfPlayer.GetUnits();
        if (units == null)
            return;

        UnitBehavior.Stance stance = currentState == StrategicState.AttackingPlayer ||
            currentState == StrategicState.AttackingHeart
            ? attackUnitStance
            : defendUnitStance;

        foreach (var unit in units)
        {
            if (unit == null)
                continue;

            unit.setFormation(defaultUnitFormation);
            unit.setStance(stance);

            if (heroFormationController != null)
            {
                EnsureUnitUsesHeroFormation(unit);
            }
            else
            {
                unit.setFollowTarget(heroTransform.gameObject);
            }
        }

        if (heroFormationController != null)
            heroFormationController.RecalculateFormations();
    }

    void EnsureUnitUsesHeroFormation(UnitBehavior unit)
    {
        GameObject heroFormationObject = heroFormationController.gameObject;
        GameObject followTarget = unit.getFollowTarget();

        if (followTarget != null && followTarget.transform.parent == heroFormationController.transform)
            return;

        unit.addToGroup(heroFormationObject);
    }

    void LookAtTarget(Vector2 targetPosition)
    {
        if (animator == null)
            return;

        Vector2 lookDirection = targetPosition - (Vector2)heroTransform.position;
        if (lookDirection.sqrMagnitude <= 0.001f)
            return;

        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);
    }

    bool ShouldCaptureShrine(Shrine shrine)
    {
        return shrine != null && shrine.Owner != selfPlayer;
    }

    bool ShouldCollectSoul(SoulOrb soul)
    {
        if (soul == null)
            return false;

        SoulResource resource = selfPlayer.GetComponent<SoulResource>();
        if (resource == null)
            return true;

        if (resource.CurrentSouls < lowSoulThreshold)
            return true;

        float distance = Vector2.Distance(heroTransform.position, soul.transform.position);
        return distance <= soulSearchRadius * 0.5f;
    }

    bool IsStrongerThanEnemy()
    {
        float myPower = CalculateArmyPower(selfPlayer);
        float enemyPower = CalculateArmyPower(enemyPlayer);

        if (enemyPower <= 0.01f)
            return myPower > 0f;

        return myPower >= enemyPower * attackPowerMultiplier;
    }

    float CalculateArmyPower(Player player)
    {
        if (player == null)
            return 0f;

        float unitPower = CountUnits(player);
        float shrinePower = CountOwnedShrines(player) * shrinePowerWeight;

        SoulResource souls = player.GetComponent<SoulResource>();
        float soulPower = souls != null ? souls.CurrentSouls * soulPowerWeight : 0f;

        return unitPower + shrinePower + soulPower;
    }

    int CountUnits(Player player)
    {
        if (player == null)
            return 0;

        int count = 0;
        List<UnitBehavior> units = player.GetUnits();
        if (units == null)
            return 0;

        foreach (var unit in units)
        {
            if (unit != null)
                count++;
        }

        return count;
    }

    int CountOwnedShrines(Player player)
    {
        if (player == null)
            return 0;

        int count = 0;

        foreach (var shrine in shrines)
        {
            if (shrine != null && shrine.Owner == player)
                count++;
        }

        return count;
    }

    Shrine FindClosestShrineToContest()
    {
        Shrine closest = null;
        float minDist = float.MaxValue;

        foreach (var shrine in shrines)
        {
            if (shrine == null || shrine.Owner == selfPlayer)
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

    Shrine FindClosestOwnedShrine(Player owner)
    {
        Shrine closest = null;
        float minDist = float.MaxValue;

        foreach (var shrine in shrines)
        {
            if (shrine == null || shrine.Owner != owner)
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

    SoulOrb FindClosestCollectibleSoul()
    {
        SoulOrb[] soulOrbs = FindObjectsByType<SoulOrb>(FindObjectsSortMode.None);
        SoulOrb closest = null;
        float minDist = float.MaxValue;

        foreach (var orb in soulOrbs)
        {
            if (orb == null || orb.Owner != selfPlayer)
                continue;

            float dist = Vector2.Distance(
                heroTransform.position,
                orb.transform.position
            );

            if (dist > soulSearchRadius)
                continue;

            if (dist < minDist)
            {
                minDist = dist;
                closest = orb;
            }
        }

        return closest;
    }

    Spell GetConvertShrineSpell()
    {
        foreach (var spell in availableSpells)
        {
            if (spell != null && spell is ConvertShrineSpell)
                return spell;
        }

        return null;
    }

    SummonSpell GetSummonSpell()
    {
        foreach (var spell in availableSpells)
        {
            if (spell != null && spell is SummonSpell summonSpell)
                return summonSpell;
        }

        return null;
    }

    bool CanCastSpell(Spell spell)
    {
        if (spell == null || spellCaster == null || heroMana == null)
            return false;

        if (!heroMana.CanAfford(spell))
            return false;

        if (spellCaster.IsOnCooldown(spell))
            return false;

        return true;
    }

    Vector2 GetSummonPosition(float castRange)
    {
        Vector2 targetPosition = heroTransform.position;
        Vector2 direction = Vector2.right;

        if (currentTargetShrine != null && currentTargetShrine.Owner != selfPlayer)
        {
            direction = ((Vector2)currentTargetShrine.transform.position - (Vector2)heroTransform.position).normalized;
        }
        else if (enemyPlayer != null)
        {
            direction = ((Vector2)enemyPlayer.transform.position - (Vector2)heroTransform.position).normalized;
        }
        else if (enemyHeart != null)
        {
            direction = ((Vector2)enemyHeart.position - (Vector2)heroTransform.position).normalized;
        }

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.right;

        float distance = Mathf.Min(summonDistance, castRange * 0.8f);
        return targetPosition + direction * distance;
    }

    void MoveHeroTo(Vector2 position)
    {
        if (attackInProgress)
            return;

        heroTransform.position = Vector2.MoveTowards(
            heroTransform.position,
            position,
            moveSpeed * Time.deltaTime
        );
    }
}

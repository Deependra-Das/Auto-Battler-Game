using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Transform unitUIContainer;
    [SerializeField] protected TMP_Text _levelText;
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider shieldBar;
    [SerializeField] protected Image shieldFillImage;
    [SerializeField] private VisualEffect _vfxParticleGraph;

    private Material _material;    
    private UnitDragHandler _unitDragHandler;

    protected UnitData unitData;
    protected Collider2D unitCollider;

    protected int totalDamage;
    protected int totalHealth;
    protected int totalShield;
    protected int totalHealing;
    protected float totalAttackSpeed;
    protected float totalAttackCoolDown;

    protected int currentHealth;
    protected int currentShield;

    protected TeamBuffData currentTeamBuffData;

    protected UnitFacingDirectionEnum directionFacing = UnitFacingDirectionEnum.Down;
    protected BaseUnit currentTarget = null;
    protected Node currentNode;

    protected bool isTargetInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= unitData.baseRange;
    protected bool isMoving;
    protected bool isAttacking;
    protected Node destination;

    protected bool isDead = false;
    protected bool canAttack = true;
    protected bool isActive = false;
    protected TeamEnum team;

    protected const float tintFadeSpeed = 5f;
    protected float tintMaxAlpha = 1f;

    protected Coroutine combatRoutine;
    protected Coroutine attackRoutine;
    protected Coroutine cooldownRoutine;
    protected Coroutine deathRoutine;
    protected Coroutine fadeTintCoroutine;

    public bool IsDead => isDead;
    public UnitData UnitData => unitData;
    public string UnitName => unitData.unitName;
    public UnitFactionEnum UnitFaction => unitData.unitFaction;
    public UnitTypeEnum UnitType => unitData.unitType;
    public TeamEnum Team => team;
    public Node CurrentNode => currentNode;
    public UnitFacingDirectionEnum DirectionFacing => directionFacing;
    public bool CanBeDragged => !isActive && !isDead;

    protected GraphService _graphServiceObj;
    protected UnitColorService _unitColorServiceObj;
    protected VfxPoolService vfxPoolServiceObj;

    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeToEvents();

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.GameplayStateChanged, OnGameplayStateChanged_BaseUnit);
        EventBusManager.Instance.Subscribe(EventNameEnum.TeamBuffUpdated, OnTeamBuffUpdated);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.GameplayStateChanged, OnGameplayStateChanged_BaseUnit);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.TeamBuffUpdated, OnTeamBuffUpdated);
    }

    protected virtual void Awake()
    {
        unitCollider = GetComponent<Collider2D>();
        _unitDragHandler = GetComponent<UnitDragHandler>();
        _material = GetComponent<SpriteRenderer>().material;
    }

    public virtual void Initialize(UnitData unitData, TeamEnum team, Node spawnNode)
    {
        _graphServiceObj = GameManager.Instance.Get<GraphService>();
        _unitColorServiceObj = GameManager.Instance.Get<UnitColorService>();
        vfxPoolServiceObj = GameManager.Instance.Get<VfxPoolService>();

        this.unitData = unitData;
        this.team = team;
        currentNode = spawnNode;
        transform.position = currentNode.worldPosition;
        currentNode.SetOccupied(true);
        _levelText.text = unitData.unitLevel.ToString();
        totalHealth = unitData.baseHealth;
        totalShield = unitData.baseShield;
        shieldFillImage.color = GetShieldColor(unitData.unitElement);
        _unitDragHandler.Initialize();
        _vfxParticleGraph.Stop();

        ResetVitals();

        currentTeamBuffData = GameManager.Instance.Get<BuffService>().GetTeamBuffData(team);
        ApplyTeamBuffs();
    }

    protected void ResetVitals()
    {
        currentHealth = totalHealth;
        currentShield = totalShield;

        healthBar.maxValue = totalHealth;
        shieldBar.maxValue = totalShield;

        UpdateHealthBar(currentHealth);
        UpdateShieldBar(currentShield);
    }

    private void StartCombatLoop()
    {
        if (combatRoutine != null) return;
     
        isActive = true;
        combatRoutine = StartCoroutine(CombatLoopCoroutine());
    }

    private IEnumerator CombatLoopCoroutine()
    {
        while (isActive)
        {
            CombatLoop();
            yield return null;
        }

        combatRoutine = null;
    }

    private void StopCombatLoop()
    {
        isActive = false;

        if (combatRoutine != null)
        {
            StopCoroutine(combatRoutine);
            combatRoutine = null;
        }
    }

    protected void CombatLoop()
    {
        HandleTargeting();
        HandleMovement();
        HandleAttack();
    }

    protected virtual void HandleTargeting()
    {
        if (currentTarget == null)
        {
            FindTarget();
        }
    }

    protected virtual void HandleMovement()
    {
        if (currentTarget == null || isAttacking) return;

        if (!isTargetInRange || isMoving)
        {
            GetInRange();
        }
    }

    protected virtual void HandleAttack()
    {

        if (!canAttack) return;
        if (!isTargetInRange || isMoving) return;

        Attack();
    }

    protected virtual void FindTarget()
    {
        var allEnemies = GameplayManager.Instance.GetOpponentTeamUnits(team);
        float minDistance = Mathf.Infinity;
        BaseUnit bestTargetEnemyUnit = null;

        foreach (BaseUnit enemy in allEnemies)
        {
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }

            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                bestTargetEnemyUnit = enemy;
            }
        }

        currentTarget = bestTargetEnemyUnit;
    }

    protected void GetInRange()
    {
         if (currentTarget == null) return;

        if (!isMoving)
        {
            GraphService graphService = GameManager.Instance.Get<GraphService>();
            destination = null;
            List<Node> availableNodes = graphService.GetNodesCloseTo(currentTarget.CurrentNode);
            availableNodes = availableNodes.OrderBy(x => Vector3.Distance(x.worldPosition, this.transform.position)).ToList();

            for (int i = 0; i < availableNodes.Count; i++)
            {
                if (!availableNodes[i].IsOccupied)
                {
                    destination = availableNodes[i];
                    break;
                }
            }
            if (destination == null)
                return;

            var path = graphService.GetShortestPath(currentNode, destination);
            if (path == null || path.Count <= 1)
                return;

            if (path[1].IsOccupied)
                return;

            path[1].SetOccupied(true);
            destination = path[1];
        }

        isMoving = !MoveTowardsNode(destination);
        if (!isMoving)
        {
            currentNode.SetOccupied(false);
            SetCurrentNode(destination);
        }
    }

    protected bool MoveTowardsNode(Node nextNode)
    {
        Vector3 direction = (nextNode.worldPosition - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        
        SetDirectionFacing(dirNormalized);

        if (direction.sqrMagnitude <= 0.005f)
        {
            transform.position = nextNode.worldPosition;
            animator.SetBool("IsWalking", false);
            return true;
        }

        animator.SetBool("IsWalking", true);

        this.transform.position += dirNormalized * unitData.baseMovementSpeed * Time.deltaTime;
        return false;
    }

    protected virtual void Attack()
    {
        Debug.Log("Base::Attack");
    }

    protected IEnumerator AttackCoolDownWaitCoroutine()
    {
        canAttack = false;
        yield return null;
        animator.ResetTrigger("Attack");
        yield return new WaitForSeconds(totalAttackCoolDown);
        canAttack = true;
    }

    protected virtual void DealDamage()
    {
        if (currentTarget == null || currentTarget.IsDead)
            return;

        currentTarget.TakeDamage(totalDamage, unitData.unitElement);
    }

    public void TakeDamage(int amount, UnitElementEnum incomingElement)
    {
        if (isDead) return;

        float damageMultiplier = GetDamageMultiplier(incomingElement);
        int damageToDeal = Mathf.RoundToInt(amount * damageMultiplier);

        if (currentShield > 0)
        {
            StartFadeTintCoroutine(_unitColorServiceObj.GetShieldDamageColor());
            AudioManager.Instance.PlayDamageShieldAudio();

            int absorbed = Mathf.Min(currentShield, damageToDeal);
            currentShield -= absorbed;
            damageToDeal -= absorbed;

            UpdateShieldBar(currentShield);
        }

        if (damageToDeal > 0)
        {
            StartFadeTintCoroutine(_unitColorServiceObj.GeHealthDamageColor());
            AudioManager.Instance.PlayDamageUnitAudio();

            currentHealth -= damageToDeal;
            UpdateHealthBar(currentHealth);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            currentTarget = null;
            destination = null;
            isMoving = false;
            animator.SetBool("IsDead", true);
            deathRoutine = StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayUnitDeathAudio();
        GameplayManager.Instance.MarkUnitDead(this);
        deathRoutine = null;
    }

    public void SetCurrentNode(Node node)
    {
        currentNode = node;
    }

    public void UpdateHealthBar(float currentHealthValue)
    {
        healthBar.value = currentHealthValue;
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        AudioManager.Instance.PlayHealAudio();
        vfxPoolServiceObj.SpawnHealingVfx(currentNode.worldPosition);
        StartFadeTintCoroutine(_unitColorServiceObj.GetHealingColor());
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, totalHealth);
        UpdateHealthBar(currentHealth);
    }

    public void UpdateShieldBar(float currentShieldValue)
    {
        shieldBar.value = currentShieldValue;
    }

    public void SetDirectionFacing(Vector3 direction)
    {
        if (direction.x > 0) 
        {
            directionFacing = UnitFacingDirectionEnum.Right;
        }
        else if (direction.x < 0) 
        {
            directionFacing = UnitFacingDirectionEnum.Left;
        }
        else if (direction.y > 0)
        {
            directionFacing = UnitFacingDirectionEnum.Up;
        }
        else if (direction.y < 0)
        {
            directionFacing = UnitFacingDirectionEnum.Down;
        }
    }

    protected void OnGameplayStateChanged_BaseUnit(object[] parameters)
    {
        if (parameters == null || parameters.Length == 0) return;

        GameplayStateEnum state = (GameplayStateEnum)parameters[0];

        switch (state)
        {
            case GameplayStateEnum.Preparation:
            case GameplayStateEnum.StageOver:
            case GameplayStateEnum.RoundOver:
                StopCombatLoop();
                break;

            case GameplayStateEnum.Combat:
                _vfxParticleGraph.Stop();
                StartCombatLoop();
                break;
        }
    }

    public void ReleaseCurrentNode()
    {
        if (currentNode != null)
            currentNode.SetOccupied(false);
    }

    public void SnapToNode(Node node)
    {
        transform.position = node.worldPosition;
        node.SetOccupied(true);
        SetCurrentNode(node);
    }

    public void OnDragCancelled(Node fallbackNode)
    {
        SnapToNode(fallbackNode);
    }

    public void UpdateUnitSpriteVisibility(bool state)
    {
        spriteRenderer.enabled = state;
    }

    public void UpdateUnitUIVisibility(bool state)
    {
        unitUIContainer.gameObject.SetActive(state);
    }

    protected void OnTeamBuffUpdated(object[] parameters)
    {
        TeamEnum buffForTeam = (TeamEnum)parameters[0];
        TeamBuffData updatedBuffData = (TeamBuffData)parameters[1];

        if (buffForTeam != team) return;

        currentTeamBuffData = updatedBuffData;
        ApplyTeamBuffs();
    }

    protected void ApplyTeamBuffs()
    {
        bool hasAnyBuff = currentTeamBuffData.attackBonus > 0f || currentTeamBuffData.shieldBonus > 0f ||
       currentTeamBuffData.hpBonus > 0f || currentTeamBuffData.attackSpeedBonus > 0f || GetElementBonus() > 0f;

        _vfxParticleGraph.Stop();

        if (hasAnyBuff)
        {
            _vfxParticleGraph.Reinit();
            _vfxParticleGraph.Play();
        }

        float attackBonusContribution = currentTeamBuffData.attackBonus;
        float elementBonusContribution = GetElementBonus() * unitData.baseElementalDamageScalingFactor;

        totalDamage = Mathf.RoundToInt(unitData.baseDamage * (1 + attackBonusContribution + elementBonusContribution));
        totalShield = unitData.baseShield + Mathf.RoundToInt(currentTeamBuffData.shieldBonus * unitData.teamShieldScalingFactor);
        totalHealth = Mathf.RoundToInt(unitData.baseHealth * (1 + currentTeamBuffData.hpBonus));
        totalAttackSpeed = unitData.baseAttackSpeed * (1 + currentTeamBuffData.attackSpeedBonus);

        totalAttackCoolDown = 1f / Mathf.Max(0.01f, totalAttackSpeed);

        ResetVitals();
    }

    protected float GetElementBonus()
    {
        return unitData.unitElement switch
        {
            UnitElementEnum.Fire => currentTeamBuffData.fireDamageBonus,
            UnitElementEnum.Thunder => currentTeamBuffData.thunderDamageBonus,
            UnitElementEnum.Nature => currentTeamBuffData.natureDamageBonus,
            _ => 0f
        };
    }

    protected float GetDamageMultiplier(UnitElementEnum attackElement)
    {
        return ElementDamageMultiplierMatrix.GetMultiplier(attackElement, unitData.unitElement);
    }

    protected Color GetShieldColor(UnitElementEnum element)
    {
        return _unitColorServiceObj.GetElementColor(element);
    }

    public void StartFadeTintCoroutine(Color color)
    {
        if (fadeTintCoroutine != null)
            StopCoroutine(fadeTintCoroutine);

        fadeTintCoroutine = StartCoroutine(FadeTintCoroutine(color));
    }

    protected IEnumerator FadeTintCoroutine(Color color)
    {
        Color current = color;
        _material.SetColor("_Tint", current);

        while (current.a > 0f)
        {
            current.a = Mathf.Clamp01(current.a - tintFadeSpeed * Time.deltaTime);
            _material.SetColor("_Tint", current);
            yield return null;
        }

        current.a = 0f;
        _material.SetColor("_Tint", current);

        fadeTintCoroutine = null;
    }

    protected void SpawnElementalVfx()
    {
        if (currentTarget == null ||  currentTarget.IsDead) return; 
        
        switch (unitData.unitElement)
        {
            case UnitElementEnum.Fire:
                vfxPoolServiceObj.SpawnFireVfx(currentTarget.CurrentNode.worldPosition);
                break;
            case UnitElementEnum.Nature:
                vfxPoolServiceObj.SpawnNatureVfx(currentTarget.CurrentNode.worldPosition);
                break;
            case UnitElementEnum.Thunder:
                vfxPoolServiceObj.SpawnThunderVfx(currentTarget.CurrentNode.worldPosition);
                break;
        }
        
    }

    public virtual void PlayFootstep()
    {
        Debug.Log("Play Footstep");
    }

    private void StopAttackLoop()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private void StopCoolDownLoop()
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }
    }

    private void StopDeathLoop()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }
    }

    public virtual void Reset()
    {
        StopCombatLoop();
        StopAttackLoop();
        StopCoolDownLoop();
        StopDeathLoop();

        currentTarget = null;
        destination = null;

        isDead = false;
        isMoving = false;
        isAttacking = false;
        isActive = false;
        canAttack = true;

        currentHealth = 0;
        currentShield = 0;

        currentNode = null;
        currentTeamBuffData = default;

        animator.Rebind();
        animator.Update(0f);

        animator.SetBool("IsDead", false);
        animator.SetBool("IsWalking", false);
        animator.ResetTrigger("Attack");

        _vfxParticleGraph.Stop();
        _vfxParticleGraph.Reinit();
    }
}
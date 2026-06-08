using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Transform unitUIContainer;
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider shieldBar;
    [SerializeField] protected Image shieldFillImage;

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

    protected Coroutine combatRoutine;

    public bool IsDead => isDead;
    public UnitData UnitData => unitData;
    public string UnitName => unitData.unitName;
    public UnitFactionEnum UnitFaction => unitData.unitFaction;
    public UnitTypeEnum UnitType => unitData.unitType;
    public TeamEnum Team => team;
    public Node CurrentNode => currentNode;
    protected bool HasEnemy => currentTarget != null;
    public UnitFacingDirectionEnum DirectionFacing => directionFacing;
    public bool CanBeDragged => !isActive && !isDead;

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
    }

    public void Initialize(UnitData unitData, TeamEnum team, Node spawnNode)
    {
        this.unitData = unitData;
        this.team = team;
        currentNode = spawnNode;
        transform.position = currentNode.position;
        currentNode.SetOccupied(true);
        totalHealth = unitData.baseHealth;
        totalShield = unitData.baseShield;
        shieldFillImage.color = GetShieldColor(unitData.unitElement);
        ResetVitals();
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
        if (!HasEnemy)
            FindTarget();
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
        if (currentTarget == null)
            return;

        if (!isMoving)
        {
            GraphService graphService = GameManager.Instance.Get<GraphService>();
            destination = null;
            List<Node> availableNodes = graphService.GetNodesCloseTo(currentTarget.CurrentNode);
            availableNodes = availableNodes.OrderBy(x => Vector3.Distance(x.position, this.transform.position)).ToList();

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
            if (path == null && path.Count <= 1)
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
        Vector3 direction = (nextNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        
        SetDirectionFacing(dirNormalized);

        if (direction.sqrMagnitude <= 0.005f)
        {
            transform.position = nextNode.position;
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

        int remainingDamage = amount;

        if (currentShield > 0)
        {
            float multiplier = GetShieldDepletionMultiplier(incomingElement);

            int shieldDamage = Mathf.Min(currentShield, Mathf.RoundToInt(remainingDamage * multiplier));
            currentShield -= shieldDamage;
            remainingDamage -= Mathf.RoundToInt(shieldDamage / multiplier);
            UpdateShieldBar(currentShield);
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
            UpdateHealthBar(currentHealth);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            currentTarget = null;
            destination = null;
            isMoving = false;
            animator.SetBool("IsDead", true);
            StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);

        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.MarkUnitDead(this);
        }
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
        transform.position = node.position;
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
        unitUIContainer.gameObject.SetActive(false);
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

    protected float GetShieldDepletionMultiplier(UnitElementEnum attackElement)
    {
        return attackElement == unitData.unitElement ? 0.5f : 1f;
    }

    protected Color GetShieldColor(UnitElementEnum element)
    {
        return element switch
        {
            UnitElementEnum.Fire => new Color(1f, 0.78f, 0.72f),
            UnitElementEnum.Thunder => new Color(0.65f, 0.9f, 1f),
            UnitElementEnum.Nature => new Color(0.78f, 1f, 0.65f),
            _ => new Color(0.95f, 0.95f, 0.95f)
        };
    }

    public virtual void Reset()
    {
        StopAllCoroutines();

        currentTarget = null;
        destination = null;

        isDead = false;
        isMoving = false;
        isActive = false;
        canAttack = true;

        currentHealth = 0;
        currentShield = 0;

        currentNode = null;
        animator.Rebind();
        animator.Update(0f);

        animator.SetBool("IsDead", false);
        animator.SetBool("IsWalking", false);
        animator.ResetTrigger("Attack");
    }
}

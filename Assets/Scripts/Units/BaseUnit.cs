using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected string characterName;
    [SerializeField] protected UnitTypeEnum unitType;
    [SerializeField] protected UnitFactionEnum unitFaction;
    [SerializeField] protected UnitElementEnum unitElement;
    [SerializeField] protected int baseDamage = 1;
    [SerializeField] protected int baseHealth = 5;
    [SerializeField] protected int baseShield = 0;
    [SerializeField] protected int baseHealing = 0;
    [SerializeField] protected float baseAttackSpeed = 1f;
    [SerializeField] protected float baseElementalDamageScalingFactor = 0.5f;
    [SerializeField] [Range(1, 5)] protected int baseRange = 1;
    [SerializeField] protected float baseMovementSpeed = 1f;

    [SerializeField] protected Transform unitUIContainer;
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider shieldBar;
    [SerializeField] private Image shieldFillImage;
    [SerializeField] protected float delayBeforeRangedAttack = 0f;

    protected Collider2D unitCollider;
    protected UnitData unitData;

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

    protected bool isTargetInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= baseRange;
    protected bool isMoving;
    protected Node destination;

    protected bool isDead = false;
    protected bool canAttack = true;
    protected bool isActive = false;
    protected TeamEnum team;

    public UnitData UnitData => unitData;
    public string CharacterName => characterName;
    public UnitFactionEnum UnitFaction => unitFaction;
    public UnitTypeEnum UnitType => unitType;
    public TeamEnum Team => team;
    public Node CurrentNode => currentNode;
    protected bool HasEnemy => currentTarget != null;
    public UnitFacingDirectionEnum DirectionFacing => directionFacing;
    public bool CanBeDragged => !isActive && !isDead;

    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeToEvents();

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.CombatStart, OnCombatStart_BaseUnit);
        EventBusManager.Instance.Subscribe(EventNameEnum.TeamBuffUpdated, OnTeamBuffUpdated);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.CombatStart, OnCombatStart_BaseUnit);
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
        unitType = unitData.unitType;
        unitFaction = unitData.unitFaction;
        this.currentNode = spawnNode;
        transform.position = currentNode.position;
        currentNode.SetOccupied(true);
        totalHealth = baseHealth;
        totalShield = baseShield;
        shieldFillImage.color = GetShieldColor(unitElement);
        ResetVitals();
    }

    private void ResetVitals()
    {
        currentHealth = totalHealth;
        currentShield = totalShield;

        healthBar.maxValue = totalHealth;
        shieldBar.maxValue = totalShield;

        UpdateHealthBar(currentHealth);
        UpdateShieldBar(currentShield);
    }

    protected void FindTarget()
    {
        var allEnemies = GameplayManager.Instance.GetOpponentTeamUnits(team);
        float minDistance = Mathf.Infinity;
        BaseUnit enemyUnit = null;
        foreach (BaseUnit enemy in allEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, this.transform.position) <= minDistance)
            {
                minDistance = Vector3.Distance(enemy.transform.position, this.transform.position);
                enemyUnit = enemy;
            }
        }

        currentTarget = enemyUnit;
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

        this.transform.position += dirNormalized * baseMovementSpeed * Time.deltaTime;
        return false;
    }

    public void SetCurrentNode(Node node)
    {
        currentNode = node;
    }

    protected virtual void Attack()
    {
        Debug.Log("Base::Attack");
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

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("IsDead", true);
            StartCoroutine(WaitForDeathAnimationCoroutine());
        }
    }

    IEnumerator WaitForDeathAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f);
        currentNode.SetOccupied(false);
        GameplayManager.Instance.MarkUnitDead(this);
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

    protected void OnCombatStart_BaseUnit(object[] parameters)
    {
        isActive = true;
    }

    public void TemporarilyReleaseNode()
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

    private void OnTeamBuffUpdated(object[] parameters)
    {
        TeamEnum buffForTeam = (TeamEnum)parameters[0];
        TeamBuffData updatedBuffData = (TeamBuffData)parameters[1];

        if (buffForTeam != team) return;

        currentTeamBuffData = updatedBuffData;
        ApplyTeamBuffs();
    }

    private void ApplyTeamBuffs()
    {
        float attackBonusContribution = currentTeamBuffData.attackBonus;
        float elementBonusContribution = GetElementBonus() * baseElementalDamageScalingFactor;

        totalDamage = Mathf.RoundToInt(baseDamage * (1 + attackBonusContribution + elementBonusContribution));
        totalShield = Mathf.RoundToInt(baseShield * (1 + currentTeamBuffData.shieldBonus));
        totalHealth = Mathf.RoundToInt(baseHealth * (1 + currentTeamBuffData.hpBonus));
        totalAttackSpeed = baseAttackSpeed * (1 + currentTeamBuffData.attackSpeedBonus);

        totalAttackCoolDown = 1f / Mathf.Max(0.01f, totalAttackSpeed);

        ResetVitals();
    }

    private float GetElementBonus()
    {
        return unitElement switch
        {
            UnitElementEnum.Fire => currentTeamBuffData.fireDamageBonus,
            UnitElementEnum.Thunder => currentTeamBuffData.thunderDamageBonus,
            UnitElementEnum.Nature => currentTeamBuffData.natureDamageBonus,
            _ => 0f
        };
    }

    private float GetShieldDepletionMultiplier(UnitElementEnum attackElement)
    {
        return attackElement == unitElement ? 0.5f : 1f;
    }

    private Color GetShieldColor(UnitElementEnum element)
    {
        return element switch
        {
            UnitElementEnum.Fire => new Color(1f, 0.78f, 0.72f),
            UnitElementEnum.Thunder => new Color(0.65f, 0.9f, 1f),
            UnitElementEnum.Nature => new Color(0.78f, 1f, 0.65f),
            _ => new Color(0.95f, 0.95f, 0.95f)
        };
    }
}

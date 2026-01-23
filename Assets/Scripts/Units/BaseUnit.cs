using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseUnit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected string characterName;
    [SerializeField] protected UnitTypeEnum unitType;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;

    [SerializeField] protected int baseDamage = 1;
    [SerializeField] protected int baseHealth = 5;
    [SerializeField] protected int baseShield = 0;
    [SerializeField] protected int baseMana = 0;
    [SerializeField] protected int baseHealing = 0;

    [SerializeField] [Range(1, 5)] protected int baseRange = 1;
    [SerializeField] protected float baseMovementSpeed = 1f;
    [SerializeField] protected float baseManaRegenSpeed = 0f;

    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider shieldBar;

    [SerializeField] protected float attackCoolDown = 1f;
    [SerializeField] protected float delayBeforeRangedAttack = 0f;

    protected int totalDamage;
    protected int totalHealth;
    protected int totalShield;
    protected int totalMana;
    protected int totalHealing;
    protected int totalRange;
    protected float totalAttackSpeed;
    protected float totalMovementSpeed;
    protected float totalManaRegenSpeed;

    protected int currentHealth;
    protected int currentShield;
    protected int currentMana;

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

    public string CharacterName => characterName;
    public UnitTypeEnum UnitType => unitType;
    public TeamEnum Team => team;
    public Node CurrentNode => currentNode;
    protected bool HasEnemy => currentTarget != null;
    public UnitFacingDirectionEnum DirectionFacing => directionFacing;

    protected bool isDragging;
    protected Vector3 dragOffset;
    protected Node originalNode;
    protected Camera mainCamera;
    protected Collider2D unitCollider;

    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeToEvents();

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.CombatStart, OnCombatStart_BaseUnit);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.CombatStart, OnCombatStart_BaseUnit);
    }
    protected virtual void Awake()
    {
        mainCamera = Camera.main;
        unitCollider = GetComponent<Collider2D>();
    }

    public void Initialize(TeamEnum team, Node spawnNode)
    {
        this.team = team;
        this.currentNode = spawnNode;
        transform.position = currentNode.position;
        currentNode.SetOccupied(true);

        InitializeHealth();
        InitializeShield();
    }

    protected void InitializeHealth()
    {
        totalHealth = baseHealth;
        currentHealth = totalHealth;
        healthBar.maxValue = totalHealth;
        UpdateHealthBar(currentHealth);
    }

    protected void InitializeShield()
    {
        totalShield = baseShield;
        currentShield = totalShield;
        shieldBar.maxValue = totalShield;
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

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        int remainingDamage = amount;

        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(currentShield, remainingDamage);
            currentShield -= shieldDamage;
            remainingDamage -= shieldDamage;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isActive || isDead) return;

        isDragging = true;
        originalNode = currentNode;
        currentNode.SetOccupied(false);
        unitCollider.enabled = false;
        Vector3 worldPos = ScreenToWorld(eventData.position);
        dragOffset = transform.position - worldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        transform.position = worldPos + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        unitCollider.enabled = true;
        Node targetNode = GetNodeUnderPointer(eventData);

        if (targetNode != null && !targetNode.IsOccupied)
        {
            SnapToNode(targetNode);
        }
        else
        {
            SnapToNode(originalNode);
        }
    }

    protected Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        Vector3 pos = screenPosition;
        pos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(pos);
    }

    protected Node GetNodeUnderPointer(PointerEventData eventData)
    {
        var go = eventData.pointerCurrentRaycast.gameObject;
        if (go == null)
            return null;

        // If raycast hit the unit itself, try parent
        Tile tile = go.GetComponent<Tile>() ?? go.GetComponentInParent<Tile>();
        if (tile == null)
            return null;

        return tile.Node;
    }

    protected void SnapToNode(Node node)
    {
        transform.position = node.position;
        node.SetOccupied(true);
        SetCurrentNode(node);
    }
}

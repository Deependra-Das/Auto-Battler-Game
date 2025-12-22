using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnit : MonoBehaviour
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
    [SerializeField] protected float baseAttackSpeed = 1f;
    [SerializeField] protected float baseMovementSpeed = 1f;
    [SerializeField] protected float baseManaRegenSpeed = 0f;

    [SerializeField] protected Slider healthBar;

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

    protected float attackCoolDown;
    protected BaseUnit currentTarget = null;
    protected Node currentNode;

    protected bool isTargetInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= baseRange;
    protected bool isMoving;
    protected Node destination;

    protected bool isDead = false;
    protected bool canAttack = true;

    protected TeamEnum _team;

    public string CharacterName => characterName;
    public UnitTypeEnum UnitType => unitType;
    public TeamEnum Team => _team;
    public Node CurrentNode => currentNode;
    protected bool HasEnemy => currentTarget != null;

    public void Initialize(TeamEnum team, Node spawnNode)
    {
        _team = team;
        this.currentNode = spawnNode;
        transform.position = currentNode.position;
        currentNode.SetOccupied(true);
       totalHealth = baseHealth;
       currentHealth = totalHealth;
        healthBar.maxValue = totalHealth;
        UpdateHealthBar(currentHealth);
    }

    protected void FindTarget()
    {
        var allEnemies = GameplayManager.Instance.GetOpponentTeamUnits(_team);
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
        if (!canAttack)
            return;

        Vector3 direction = (currentTarget.CurrentNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);

        animator.SetTrigger("Attack");
        attackCoolDown = 1 / baseAttackSpeed;
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        canAttack = false;
        yield return null;
        animator.ResetTrigger("Attack");
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthBar(currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("IsDead", true);
            currentNode.SetOccupied(false);
            GameplayManager.Instance.MarkUnitDead(this);
        }
    }

    public void UpdateHealthBar(float currentHealthValue)
    {
        healthBar.value = currentHealthValue;
    }
}

using AutoBattler.Main;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] private string _characterName;
    [SerializeField] private UnitTypeEnum _unitType;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    [SerializeField] private int _baseDamage = 1;
    [SerializeField] private int _baseHealth = 5;
    [SerializeField] private int _baseShield = 0;
    [SerializeField] private int _baseMana = 0;
    [SerializeField] private int _baseHealing = 0;

    [SerializeField] [Range(1, 5)] private int baseRange = 1;
    [SerializeField] private float _baseAttackSpeed = 1f;
    [SerializeField] private float _baseMovementSpeed = 1f;
    [SerializeField] private float _baseManaRegenSpeed = 0f;

    private float _damageMultiplier = 1f;
    private float _healthMultiplier = 1f;
    private float _shieldMultiplier = 1f;
    private float _manaMultiplier = 1f;
    private float _healingMultiplier = 1f;
    private float _rangeMultiplier = 1f;
    private float _attackSpeedMultiplier = 1f;
    private float _manaRegenMultiplier = 1f;

    protected float attackCoolDown;
    protected BaseUnit currentTarget = null;
    protected Node currentNode;

    protected bool isTargetInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= baseRange;
    protected bool isMoving;
    protected Node destination;

    protected bool isDead = false;
    protected bool canAttack = true;

    private TeamEnum _team;

    public string CharacterName => _characterName;
    public UnitTypeEnum UnitType => _unitType;
    public TeamEnum Team => _team;
    public Node CurrentNode => currentNode;
    protected bool HasEnemy => currentTarget != null;

    public void Initialize(TeamEnum team, Node spawnNode)
    {
        _team = team;
        this.currentNode = spawnNode;
        transform.position = currentNode.position;
        currentNode.SetOccupied(true);
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
            List<Node> candidates = graphService.GetNodesCloseTo(currentTarget.CurrentNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.position, this.transform.position)).ToList();
            for (int i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsOccupied)
                {
                    destination = candidates[i];
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
    }
}

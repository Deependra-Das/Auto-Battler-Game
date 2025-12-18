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


    void Start()
    {
        
    }
}

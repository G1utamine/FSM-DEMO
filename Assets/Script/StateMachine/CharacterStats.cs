using UnityEngine;

[System.Serializable]
public class CharacterStats : IPlayerStats
{
    [Header("水平移动")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float airMoveSpeed = 6f;
    [SerializeField] private float dashSpeed = 10f;

    [Header("垂直移动")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float wallSlideSpeed = 5f;

    [Header("属性")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float attackColdTime = 1f;

    // 运行时状态（不需要在Inspector里序列化）
    private float _maxFallSpeed = 30f;
    private int _currentHealth;
    private bool _isHurt;

    // ICharacterStats 实现
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float AirMoveSpeed => airMoveSpeed;
    public float JumpForce => jumpForce;
    public int MaxHealth => maxHealth;

    public float MaxFallSpeed
    {
        get => _maxFallSpeed;
        set => _maxFallSpeed = value;
    }
    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    public bool IsHurt
    {
        get => _isHurt;
        set => _isHurt = value;
    }

    // IPlayerStats 实现
    public float DashSpeed => dashSpeed;
    public float WallSlideSpeed => wallSlideSpeed;
    public float AttackColdTime => attackColdTime;

    public void Init()
    {
        _currentHealth = maxHealth;
        _maxFallSpeed = 0f;
        _isHurt = false;
    }
}
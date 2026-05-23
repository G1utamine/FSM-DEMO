public interface ICharacterStats
{
    // 水平移动
    float WalkSpeed { get; }
    float RunSpeed { get; }
    float AirMoveSpeed { get; }

    // 垂直移动
    float JumpForce { get; }
    float MaxFallSpeed { get; set; }

    // 属性
    int MaxHealth { get; }
    int CurrentHealth { get; set; }
    bool IsHurt { get; set; }
}
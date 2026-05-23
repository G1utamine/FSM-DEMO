public interface IPlayerStats : ICharacterStats
{
    float DashSpeed { get; }
    float WallSlideSpeed { get; }
    float AttackColdTime { get; }
}
public interface ICharacterInput
{
    float MoveX { get; }
    float RawMoveX { get; }
    bool Jump { get; }
    bool Dash { get; }
    bool Attack { get; }
    bool Run { get; }
}
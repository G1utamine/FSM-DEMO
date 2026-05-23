public class FallState : BaseState
{
    public FallState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        context.Animator.Play("ToFall");
        acceleration = 30f;
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        ChangeAnimation("Falling");
        Flip();
    }

    public override void OnPhysicsLogic()
    {
        SetXSpeed(context.Input.MoveX * context.Stats.AirMoveSpeed);
    }
}
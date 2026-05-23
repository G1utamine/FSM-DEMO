public class WalkState : BaseState
{
    public WalkState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        switch (context.LastState)
        {
            case RunState:
                context.Animator.Play("BreakRun");
                acceleration = 50f;
                break;
            case DashState:
                context.Animator.Play("BreakDash");
                acceleration = 25f;
                break;
            default:
                context.Animator.Play("ToWalk");
                acceleration = 100f;
                break;
        }
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        ChangeAnimation("Walking");
        Flip();
    }

    public override void OnPhysicsLogic()
    {
        SetXSpeed(context.Input.MoveX * context.Stats.WalkSpeed);
    }
}
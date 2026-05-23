using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        switch (context.LastState)
        {
            case WalkState:
                context.Animator.Play("BreakWalk");
                acceleration = 100f;
                break;
            case RunState:
                context.Animator.Play("BreakRun");
                acceleration = 50f;
                break;
            case JumpState:
            case FallState:
                if (context.Stats.MaxFallSpeed >= 22f)
                {
                    context.Animator.Play("LandHard");
                    context.Stats.MaxFallSpeed = 0;
                }
                else
                    context.Animator.Play("Land");
                acceleration = 100f;
                break;
            case DashState:
                context.Animator.Play("BreakDash");
                acceleration = 30f;
                break;
            default:
                context.Animator.Play("Idle");
                acceleration = 100f;
                break;
        }
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        ChangeAnimation("Idle");
    }

    public override void OnPhysicsLogic()
    {
        SetXSpeed(0);
    }
}
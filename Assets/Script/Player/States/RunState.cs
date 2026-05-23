using UnityEngine;

public class RunState : BaseState
{
    public RunState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        switch (context.LastState)
        {
            case DashState:
                context.Animator.Play("BreakDash");
                acceleration = 25f;
                break;
            default:
                context.Animator.Play("ToRun");
                acceleration = 50f;
                break;
        }
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        ChangeAnimation("Running");
        FlipWithTurnAnim();
    }

    public override void OnPhysicsLogic()
    {
        SetXSpeed(context.Input.MoveX * context.Stats.RunSpeed);
    }

    // 跑步专属：转身时播放转身动画
    private void FlipWithTurnAnim()
    {
        float moveDir = context.Input.MoveX;
        float facingDir = context.CharacterTransform.localScale.x;

        if (Mathf.Abs(moveDir) > 0.01f && Mathf.Sign(moveDir) != Mathf.Sign(facingDir))
        {
            context.Animator.Play("RunTrickTurn");
        }

        Flip();
    }
}
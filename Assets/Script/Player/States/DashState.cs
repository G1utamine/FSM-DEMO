using UnityEngine;

public class DashState : BaseState
{
    public DashState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        context.Animator.Play("ToDash");
        acceleration = 50f;
        context.StateLocked = true;
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        ChangeAnimation("Dashing");
        AnimatorStateInfo current = context.Animator.GetCurrentAnimatorStateInfo(0);
        if (current.normalizedTime >= 1.0f && current.IsName("Dashing"))
            context.StateLocked = false;
    }

    public override void OnPhysicsLogic()
    {
        IPlayerStats stats = GetPlayerStats();
        SetXSpeed(acceleration * stats.DashSpeed * context.CharacterTransform.localScale.x);
    }
}
using UnityEngine;

public class HurtState : BaseState
{
    public HurtState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        context.Animator.Play("Hurt");
        acceleration = 50f;
        context.StateLocked = true;
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        AnimatorStateInfo current = context.Animator.GetCurrentAnimatorStateInfo(0);
        if (current.normalizedTime >= 1.0f && current.IsName("Hurt"))
        {
            context.StateLocked = false;
            context.Stats.IsHurt = false;
        }
    }

    public override void OnPhysicsLogic()
    {
        float knockback = acceleration * context.Stats.WalkSpeed * 0.01f;
        SetXSpeed(knockback * -context.CharacterTransform.localScale.x);
    }
}
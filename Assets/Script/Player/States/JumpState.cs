using UnityEngine;

public class JumpState : BaseState
{
    private int animValue;

    public JumpState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        context.Animator.Play("ToJump");
        animValue = Random.Range(0, 2);
        acceleration = 30f;
        context.Rigidbody.velocity = new Vector2(
            context.Rigidbody.velocity.x,
            context.Stats.JumpForce
        );
    }

    public override void OnExit() { }

    public override void OnLogic()
    {
        ChangeAnimation(animValue == 1 ? "Jumping01" : "Jumping02");
        Flip();
    }

    public override void OnPhysicsLogic()
    {
        SetXSpeed(context.Input.MoveX * context.Stats.AirMoveSpeed);
    }
}
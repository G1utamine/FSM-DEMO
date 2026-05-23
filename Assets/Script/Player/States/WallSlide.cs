using UnityEngine;

public class WallSlide : BaseState
{
    public WallSlide(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        // 贴墙时强制面向墙壁（反转朝向）
        float flipped = -context.CharacterTransform.localScale.x;
        context.CharacterTransform.localScale = new Vector3(flipped, 1, 1);
        context.Animator.Play("ToWallSlide");
        acceleration = 30f;
    }

    public override void OnExit()
    {
        // 离开墙壁时恢复朝向
        float flipped = -context.CharacterTransform.localScale.x;
        context.CharacterTransform.localScale = new Vector3(flipped, 1, 1);
    }

    public override void OnLogic()
    {
        ChangeAnimation("WallSliding");
    }

    public override void OnPhysicsLogic()
    {
        IPlayerStats stats = GetPlayerStats();
        float targetY = -stats.WallSlideSpeed;
        float newY = Mathf.MoveTowards(
            context.Rigidbody.velocity.y,
            targetY,
            acceleration * Time.fixedDeltaTime
        );
        context.Rigidbody.velocity = new Vector2(0, newY);
    }
}
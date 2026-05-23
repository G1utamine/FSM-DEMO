using UnityEngine;

public abstract class BaseState : IState
{
    protected ICharacterContext context { get; private set; }

    protected BaseState(ICharacterContext context)
    {
        this.context = context;
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnPhysicsLogic();
    public abstract void OnLogic();

    protected float acceleration;

    // Flip 统一交给 Context 处理，BaseState 只负责调用
    protected void Flip()
    {
        context.Flip();
    }

    // 等待过渡动画结束后切换到循环动画
    protected void ChangeAnimation(string animName)
    {
        AnimatorStateInfo current = context.Animator.GetCurrentAnimatorStateInfo(0);
        if (current.normalizedTime >= 1.0f && !current.IsName(animName))
        {
            context.Animator.Play(animName);
        }
    }

    // 平滑设置X方向速度
    protected void SetXSpeed(float targetSpeed)
    {
        float newX = Mathf.MoveTowards(
            context.Rigidbody.velocity.x,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );
        context.Rigidbody.velocity = new Vector2(newX, context.Rigidbody.velocity.y);
    }

    // 访问玩家专属Stats的辅助方法，转型失败会给出明确报错
    protected IPlayerStats GetPlayerStats()
    {
        if (context.Stats is IPlayerStats playerStats)
            return playerStats;

        throw new System.InvalidCastException(
            $"{GetType().Name} 需要 IPlayerStats，但 Context 提供的是 {context.Stats.GetType().Name}"
        );
    }
}
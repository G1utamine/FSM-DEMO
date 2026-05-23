using UnityEngine;

public class ComboAttackState : BaseState
{
    private int combo = 0;
    private bool canCombo = false;

    public ComboAttackState(ICharacterContext context) : base(context) { }

    public override void OnEnter()
    {
        acceleration = 50f;
        context.StateLocked = true;

        combo++;
        if (combo > 3) combo = 1;

        PlayComboAnim(combo);
        canCombo = false;
    }

    public override void OnExit()
    {
        canCombo = false;
    }

    public override void OnLogic()
    {
        AnimatorStateInfo current = context.Animator.GetCurrentAnimatorStateInfo(0);
        bool isCurrentAnim = current.IsName(GetAnimName(combo));

        // 进入可续招窗口
        if (isCurrentAnim && current.normalizedTime >= 0.6f)
            canCombo = true;

        // 续招
        if (canCombo && context.Input.Attack)
        {
            combo++;
            if (combo > 3) combo = 1;

            PlayComboAnim(combo);
            canCombo = false;
            return;
        }

        // 退出
        if (isCurrentAnim && current.normalizedTime >= 1.0f)
        {
            combo = 0;
            context.StateLocked = false;
        }
    }

    public override void OnPhysicsLogic()
    {
        SetXSpeed(0);
    }

    private void PlayComboAnim(int index)
    {
        context.Animator.Play(GetAnimName(index));
    }

    private string GetAnimName(int index)
    {
        switch (index)
        {
            case 1: return "LightAttack";
            case 2: return "HeavyAttack";
            case 3: return "GuardedAttack";
            default: return "LightAttack";
        }
    }
}
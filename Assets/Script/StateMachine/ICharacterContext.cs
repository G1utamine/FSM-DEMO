using UnityEngine;

public interface ICharacterContext
{
    // 状态机
    IState LastState { get; }

    // 组件
    Transform CharacterTransform { get; }
    Animator Animator { get; }
    Rigidbody2D Rigidbody { get; }

    // 抽象接口（不再绑定具体类）
    ICharacterStats Stats { get; }
    ICharacterInput Input { get; }

    // 状态锁
    bool StateLocked { get; set; }

    // 通用方法（统一管理，不散落在各State里）
    void Flip();
}
# Unity FSM Framework · 2D角色状态机框架

一个轻量、可复用的 Unity 2D 角色行为状态机框架，通过接口抽象将状态机核心与具体角色实现完全解耦，支持玩家与 AI 角色共用同一套状态机。

---

## 特性

- **完全解耦** — 状态机核心不依赖任何具体角色类，通过 `ICharacterContext` / `ICharacterInput` / `ICharacterStats` 接口与角色通信
- **泛型状态注册** — 基于 `Dictionary<Type, IState>` 实现状态动态注册与统一调度
- **Func 条件驱动** — 所有状态切换通过 `Func<bool>` 委托声明，切换逻辑集中在 Controller，状态类本身不互相依赖
- **AnyState 机制** — 支持跳跃、冲刺等高优先级行为在任意状态下触发，按注册顺序决定优先级
- **代码驱动动画** — 摒弃 Animator 连线，通过代码控制动画过渡，降低状态增加时的维护成本

---

## 项目结构

```
Assets/Scripts/
├── StateMachine/              # 框架核心（与具体角色无关）
│   ├── IState.cs              # 状态接口
│   ├── IStateMachine.cs       # 状态机接口
│   ├── ICharacterContext.cs   # 角色上下文接口
│   ├── ICharacterInput.cs     # 输入接口
│   ├── ICharacterStats.cs     # 通用属性接口
│   ├── IPlayerStats.cs        # 玩家专属属性接口（继承自 ICharacterStats）
│   ├── BaseState.cs           # 状态基类（提供动画、物理等通用方法）
│   ├── StateMachine.cs        # 状态机实现
│   └── CharacterStats.cs      # 玩家属性实现
│
└── Player/                    # 玩家实现（使用框架的示例）
    ├── PlayerController.cs    # 玩家控制器，实现 ICharacterContext
    ├── Input/
    │   └── PlayerInput.cs     # 玩家输入，实现 ICharacterInput
    └── States/                # 9 种行为状态
        ├── IdleState.cs
        ├── WalkState.cs
        ├── RunState.cs
        ├── JumpState.cs
        ├── FallState.cs
        ├── DashState.cs
        ├── WallSlide.cs
        ├── HurtState.cs
        └── ComboAttackState.cs
```

---

## 快速上手

### 1. 实现接口

```csharp
// 自定义输入（玩家键盘 / 敌人 AI 均可）
public class PlayerInput : ICharacterInput
{
    public float MoveX { get; private set; }
    public bool Jump { get; private set; }
    // ...

    public void Update()
    {
        MoveX = Input.GetAxisRaw("Horizontal");
        Jump = Input.GetKeyDown(KeyCode.Space);
        // ...
    }
}

// 角色控制器实现 ICharacterContext
public class PlayerController : MonoBehaviour, ICharacterContext
{
    private IStateMachine fsm;
    public ICharacterInput Input => input;
    public ICharacterStats Stats => characterStats;
    // ...
}
```

### 2. 注册状态

```csharp
private void RegisterStates()
{
    fsm.AddState<IdleState>(new IdleState(this));
    fsm.AddState<JumpState>(new JumpState(this));
    // 继续添加...
}
```

### 3. 声明切换条件

```csharp
private void RegisterTransitions()
{
    // 普通切换
    fsm.AddTransition<IdleState, JumpState>(() => input.Jump && isGrounded);
    fsm.AddTransition<IdleState, WalkState>(() => input.MoveX != 0);

    // AnyState 高优先级切换（任意状态均可触发）
    fsm.AddAnyTransition<JumpState>(() => input.Jump && isGrounded);
    fsm.AddAnyTransition<DashState>(() => input.Dash && isGrounded);
}
```

### 4. 驱动状态机

```csharp
private void Update()   => fsm.OnLogic();
private void FixedUpdate() => fsm.OnPhysicsLogic();
```

---

## 扩展到新角色

只需实现三个接口，状态机核心代码**零改动**：

| 接口 | 说明 |
|------|------|
| `ICharacterInput` | 自定义输入来源（键盘 / AI 决策 / 网络同步） |
| `ICharacterStats` | 自定义角色属性 |
| `ICharacterContext` | 将组件与接口组装，挂到角色 GameObject 上 |

```csharp
// 敌人 AI 示例
public class EnemyAIInput : ICharacterInput
{
    public float MoveX => aiDecidedDirection;
    public bool Jump => false;
    // ...
}

public class EnemyController : MonoBehaviour, ICharacterContext
{
    public ICharacterInput Input => aiInput;
    // ...
}
```

---

## 设计说明

### 为什么不用 Animator Controller 连线？

Animator 连线在状态少时方便，但随着状态增加，连线数量以 O(n²) 增长，维护成本急剧上升。本框架将过渡逻辑完全移到代码层，新增状态只需添加状态类和切换条件，不需要改动任何现有状态。

### AnyState 优先级

`AddAnyTransition` 按注册顺序决定优先级，先注册的先检查。建议将优先级最高的行为（如受伤、死亡）最先注册。

### StateLocked 机制

部分状态（冲刺、攻击、受伤）在动画播放完毕前会锁定状态切换，防止动画被打断。`StateLocked` 由状态自身在 `OnEnter` / `OnLogic` 中管理，动画结束后自动解锁。

---

## 环境

- Unity 2021.3 LTS 及以上
- C# 9.0+
- 不依赖任何第三方插件

---

## License

MIT

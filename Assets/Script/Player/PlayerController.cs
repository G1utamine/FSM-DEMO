using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacterContext
{
    // 状态机
    private IStateMachine fsm;

    // 组件
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInput input = new PlayerInput();

    // 角色属性
    public CharacterStats characterStats;

    // 物理检测结果
    public bool isGrounded;
    public bool isWalled;
    private bool stateLocked;

    // 物理检测配置
    public Transform groundTrans;
    public Vector2 groundCheckSize;
    public Transform wallTransLeft;
    public Transform wallTransRight;
    public Vector2 wallCheckSize;
    public LayerMask groundLayerMask;

    // ICharacterContext 实现
    public IState LastState => fsm.LastState;
    public Transform CharacterTransform => transform;
    public Animator Animator => anim;
    public Rigidbody2D Rigidbody => rb;
    public ICharacterStats Stats => characterStats;
    public ICharacterInput Input => input;
    public bool StateLocked
    {
        get => stateLocked;
        set => stateLocked = value;
    }

    // Flip 统一在 Controller 里实现
    public void Flip()
    {
        float moveDir = input.MoveX;
        float facingDir = transform.localScale.x;

        if (moveDir > 0 && facingDir < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir < 0 && facingDir > 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        fsm = new StateMachine();
    }

    private void Start()
    {
        characterStats.Init();
        RegisterStates();
        RegisterTransitions();
        fsm.Init<IdleState>();
    }

    private void Update()
    {
        if (characterStats.CurrentHealth > 0)
        {
            PhysicsCheck();
            input.Update();
            TrackMaxFallSpeed();
            fsm.OnLogic();
        }
        else
        {
            anim.Play("Death");
        }
    }

    private void FixedUpdate()
    {
        if (characterStats.CurrentHealth > 0)
            fsm.OnPhysicsLogic();
    }

    private void RegisterStates()
    {
        fsm.AddState<IdleState>(new IdleState(this));
        fsm.AddState<WalkState>(new WalkState(this));
        fsm.AddState<RunState>(new RunState(this));
        fsm.AddState<JumpState>(new JumpState(this));
        fsm.AddState<FallState>(new FallState(this));
        fsm.AddState<DashState>(new DashState(this));
        fsm.AddState<WallSlide>(new WallSlide(this));
        fsm.AddState<HurtState>(new HurtState(this));
        fsm.AddState<ComboAttackState>(new ComboAttackState(this));
    }

    private void RegisterTransitions()
    {
        // Idle
        fsm.AddTransition<IdleState, WalkState>(() => input.MoveX != 0 && !input.Run);
        fsm.AddTransition<IdleState, RunState>(() => input.MoveX != 0 && input.Run);
        fsm.AddTransition<IdleState, ComboAttackState>(() => isGrounded && input.Attack);
        fsm.AddTransition<IdleState, HurtState>(() => characterStats.CurrentHealth > 0 && characterStats.IsHurt);

        // Walk
        fsm.AddTransition<WalkState, IdleState>(() => input.MoveX == 0);
        fsm.AddTransition<WalkState, RunState>(() => input.MoveX != 0 && input.Run);
        fsm.AddTransition<WalkState, ComboAttackState>(() => isGrounded && input.Attack);
        fsm.AddTransition<WalkState, HurtState>(() => characterStats.CurrentHealth > 0 && characterStats.IsHurt);

        // Run
        fsm.AddTransition<RunState, IdleState>(() => input.MoveX == 0);
        fsm.AddTransition<RunState, WalkState>(() => input.MoveX != 0 && !input.Run);
        fsm.AddTransition<RunState, ComboAttackState>(() => isGrounded && input.Attack);
        fsm.AddTransition<RunState, HurtState>(() => characterStats.CurrentHealth > 0 && characterStats.IsHurt);

        // Jump
        fsm.AddTransition<JumpState, IdleState>(() => isGrounded && rb.velocity.y <= 0);
        fsm.AddTransition<JumpState, HurtState>(() => characterStats.CurrentHealth > 0 && characterStats.IsHurt);

        // Fall
        fsm.AddTransition<FallState, IdleState>(() => isGrounded);
        fsm.AddTransition<FallState, HurtState>(() => characterStats.CurrentHealth > 0 && characterStats.IsHurt);

        // Dash
        fsm.AddTransition<DashState, IdleState>(() => input.MoveX == 0 && !stateLocked);
        fsm.AddTransition<DashState, WalkState>(() => input.MoveX != 0 && !input.Run && !stateLocked);
        fsm.AddTransition<DashState, RunState>(() => input.MoveX != 0 && input.Run && !stateLocked);

        // WallSlide
        fsm.AddTransition<WallSlide, FallState>(() => (!isWalled && !isGrounded) || input.MoveX != 0);
        fsm.AddTransition<WallSlide, IdleState>(() => isGrounded);
        fsm.AddTransition<WallSlide, HurtState>(() => characterStats.CurrentHealth > 0 && characterStats.IsHurt);

        // ComboAttack
        fsm.AddTransition<ComboAttackState, IdleState>(() => input.MoveX == 0 && !stateLocked);
        fsm.AddTransition<ComboAttackState, WalkState>(() => input.MoveX != 0 && !input.Run && !stateLocked);
        fsm.AddTransition<ComboAttackState, RunState>(() => input.MoveX != 0 && input.Run && !stateLocked);

        // Hurt
        fsm.AddTransition<HurtState, IdleState>(() => !stateLocked && isGrounded);
        fsm.AddTransition<HurtState, FallState>(() => !stateLocked && !isGrounded);

        // AnyState
        fsm.AddAnyTransition<WallSlide>(() => !isGrounded && isWalled && input.MoveX == 0 && rb.velocity.y < -0.1f);
        fsm.AddAnyTransition<JumpState>(() => input.Jump && isGrounded);
        fsm.AddAnyTransition<FallState>(() => !isGrounded && rb.velocity.y < -0.1f && !isWalled);
        fsm.AddAnyTransition<DashState>(() => isGrounded && input.Dash);
    }

    private void TrackMaxFallSpeed()
    {
        if (!isGrounded && rb.velocity.y < -0.1f)
        {
            float current = Mathf.Abs(rb.velocity.y);
            if (current > characterStats.MaxFallSpeed)
                characterStats.MaxFallSpeed = current;
        }
    }

    private void PhysicsCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundTrans.position, groundCheckSize, 0, groundLayerMask);
        bool isLeftWalled = Physics2D.OverlapBox(wallTransLeft.position, wallCheckSize, 0, groundLayerMask);
        bool isRightWalled = Physics2D.OverlapBox(wallTransRight.position, wallCheckSize, 0, groundLayerMask);
        isWalled = isLeftWalled || isRightWalled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundTrans.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallTransLeft.position, wallCheckSize);
        Gizmos.DrawWireCube(wallTransRight.position, wallCheckSize);
    }
}
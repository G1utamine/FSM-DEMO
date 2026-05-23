using UnityEngine;

public class PlayerInput : ICharacterInput
{
    private bool _jump;
    private bool _dash;
    private bool _attack;
    private bool _run;
    private float _moveX;
    private float _rawMoveX;
    private float _noInputTimer;
    private const float IdleDelay = 0.08f;

    public float MoveX => _moveX;
    public float RawMoveX => _rawMoveX;
    public bool Jump => _jump;
    public bool Dash => _dash;
    public bool Attack => _attack;
    public bool Run => _run;

    public void Update()
    {
        _attack = Input.GetKeyDown(KeyCode.Mouse0);
        _jump = Input.GetKeyDown(KeyCode.Space);
        _dash = Input.GetKeyDown(KeyCode.Q);
        _run = Input.GetKey(KeyCode.LeftShift);
        _rawMoveX = Input.GetAxisRaw("Horizontal");

        if (_rawMoveX != 0)
        {
            _moveX = _rawMoveX;
            _noInputTimer = 0f;
        }
        else
        {
            _noInputTimer += Time.deltaTime;
            if (_noInputTimer >= IdleDelay)
                _moveX = 0;
        }
    }
}
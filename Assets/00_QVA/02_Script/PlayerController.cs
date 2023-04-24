using UnityEngine;

/// <summary>
/// Playerの移動制御
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField]
    private float moveSpeed = 100f;

    // 水平入力値
    private float _hAxis;
    // 垂直入力値
    private float _vAxis;
    // Walkボタンが、押されたかどうか
    private bool _walkDown;
    private bool _JumpDown;
    // ジャンプ中？
    private bool _isJump;
    // 回避中？
    private bool _isDodge;
    
    // 移動Vector
    private Vector3 _moveVec;
    // 回避Vector (回避アクション中の方向移動を禁じるため)
    private Vector3 _dodgeVec;
    
    // アニメーター
    private Animator _animator;
    // Rigidbody
    private Rigidbody _rigidbody;
    // 移動Vector 事前正規化
    private static readonly Vector3 NormalizedVector3 = new Vector3(0f, 0f, 1f);

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() => Init();

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update ()
    {
        GetInput();
        Move();
        Rotation();
        Jump();
        Dodge();
    }

    /// <summary>
    /// キー入力
    /// </summary>
    private void GetInput()
    {
        // 方向入力を受ける
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        
        // 回避中は回避方向にのみ移動
        if (_isDodge)
            _moveVec = _dodgeVec;
        _moveVec = (_hAxis * Vector3.right + _vAxis * Vector3.forward).normalized;
        
        // Walkボタンの入力を受ける
        _walkDown = Input.GetButton("Walk");
        _JumpDown= Input.GetButtonDown("Jump");
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        // ウォーキング/ランニング速度の計算
        float speed = _walkDown ? moveSpeed * 0.5f : moveSpeed;
        
        // Player 移動処理
        _rigidbody.MovePosition(transform.position + _moveVec * (speed * Time.deltaTime));
        
        // 移動アニメ
        _animator.SetBool("isRun", _moveVec != Vector3.zero);
        _animator.SetBool("isWalk", _walkDown);
    }

    /// <summary>
    /// 回転
    /// </summary>
    private void Rotation()
    {
        // 移動方向に回転
        if (_moveVec != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(_moveVec, Vector3.up);
            _rigidbody.MoveRotation(rotation);
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void Jump()
    {
        if (_JumpDown && !_isJump && _moveVec == Vector3.zero && !_isDodge)
        {
            _rigidbody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            _animator.SetTrigger("doJump");
            _isJump = true;
        }
    }
    
    /// <summary>
    /// 回避
    /// </summary>
    private void Dodge()
    {
        if (_JumpDown && !_isJump && _moveVec != Vector3.zero && !_isDodge)
        {
            // 回避した方向取得
            _dodgeVec = _moveVec;
            // 回避時には2倍速
            moveSpeed *= 2;
            
            _animator.SetTrigger("doRoll");
            _isDodge = true;
            
            // 0.4秒後に、回避終了
            Invoke("DodgeOut", 0.4f);
        }
    }

    /// <summary>
    /// 回避終了
    /// </summary>
    private void DodgeOut()
    {
        moveSpeed *= 0.5f;
        _isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
            _isJump = false;
    }
}

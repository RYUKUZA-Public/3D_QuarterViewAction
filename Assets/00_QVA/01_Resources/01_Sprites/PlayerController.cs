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
    public float moveSpeed = 15f;

    // 水平入力値
    private float _hAxis;
    // 垂直入力値
    private float _vAxis;
    // Walkボタンが、押されたかどうか
    private bool _walkDown;
    // 移動Vector
    private Vector3 _moveVec;
    // アニメーター
    private Animator _animator;
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
    
    private void FixedUpdate ()
    {
        // 方向入力を受ける
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        _moveVec = (_hAxis * Vector3.right + _vAxis * Vector3.forward).normalized;
        
        // Walkボタンの入力を受ける
        _walkDown = Input.GetButton("Walk");
        
        // ウォーキング/ランニング速度の計算
        float speed = _walkDown ? moveSpeed * 0.5f : moveSpeed;
        
        // Player 移動処理
        _rigidbody.MovePosition(transform.position + _moveVec * (speed * Time.fixedDeltaTime));
        
        // 移動アニメ
        _animator.SetBool("isRun", _moveVec != Vector3.zero);
        _animator.SetBool("isWalk", _walkDown);
        
        // 移動方向に回転
        if (_moveVec != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(_moveVec, Vector3.up);
            _rigidbody.MoveRotation(rotation);
        }
    }
}

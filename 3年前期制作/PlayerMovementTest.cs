using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerMovementTest : MonoBehaviour
{
    [SerializeField, Header("ジャンプ力")]
    private float m_JumpPower = 4.5f;
    [SerializeField, Header("移動速度")]
    private float m_MoveSpeed = 3.0f;
    [SerializeField, Header("ジャンプまでのインターバル")]
    private float m_JumpInterval = 1.0f;

    private Vector2 m_InputVelocity = Vector2.zero;
    private RRCharacterAnimator m_CharacterAnimator = default;
    private PlayerAnimationEvents m_AnimationEvent = default;
    private FloorObjectFollowView m_View = default;
    private Rigidbody m_Rigidbody = default;
    private GoalObject m_Goal = default;
    private float m_JumpTimer = 0.0f;
    private bool m_CanJump = false;
    private bool m_IsJump = false;
    private bool m_IsMove = false;

    public uint MoveSwitch { get; set; }
    /// <summary> プレイヤーが地面に着いているか </summary>
    public bool OnGround { get; set; }
    /// <summary> プレイヤーが死んでいるか </summary>
    public bool IsDead { get; set; }

    private void Start()
    {
        m_CharacterAnimator = GetComponentInChildren<RRCharacterAnimator>();
        m_AnimationEvent = GetComponentInChildren<PlayerAnimationEvents>();
        m_View = GetComponent<FloorObjectFollowView>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Goal = GameObject.Find("ObjGoal").GetComponent<GoalObject>();
        m_JumpTimer = m_JumpInterval;
        m_CanJump = false;
        m_IsJump = false;
        m_IsMove = false;
        OnGround = false;
        PlayerMove();
        PlayerJump();
        LookGoal();
    }

    private void Update()
    {
        if (m_View.IsFollowTarget) m_View.UpdateFollowDirection();

        if (m_JumpTimer < m_JumpInterval) m_JumpTimer += Time.deltaTime;
        m_CanJump = OnGround && !m_AnimationEvent.IsPlayerMotion && m_JumpTimer >= m_JumpInterval && !IsDead && !m_Goal.IsGoal;
    }

    private void FixedUpdate()
    { //Rigidbodyを使う処理のみ
        if (IsDead || m_Goal.IsGoal || m_AnimationEvent.IsPlayerMotion) 
        {
            m_CharacterAnimator.IsJumping = false;
            m_CharacterAnimator.IsFalling = false;
            m_CharacterAnimator.MovementSpeed(0.0f);
            return;
        }

        m_CharacterAnimator.IsJumping = !OnGround;
        m_CharacterAnimator.IsFalling = !OnGround; 

        if (m_IsJump)
        {
            m_Rigidbody.velocity = transform.up * m_JumpPower;
            m_IsJump = false;
        }

        if (m_IsMove) MoveUpdate();
        else m_CharacterAnimator.MovementSpeed(0.0f);
    }

    private void MoveUpdate()
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * m_InputVelocity.y + Camera.main.transform.right * m_InputVelocity.x;
        //プレイヤーキャラクターの座標を変える
        m_Rigidbody.position += moveForward * m_MoveSpeed * Time.deltaTime * MoveSwitch;
        //プレイヤーキャラクターの向きを移動方向に向ける
        transform.rotation = Quaternion.LookRotation(moveForward);

        m_CharacterAnimator.MovementSpeed(m_MoveSpeed);
    }

    private void PlayerMove()
    {
        AbstractInputSystem.Instance.PlayerInput.VelocityObservable
            .Subscribe(v => {
                m_IsMove = (v == Vector2.zero) ? false : true;
                m_InputVelocity = v;
            }).AddTo(this);
    }

    private void PlayerJump()
    {
        AbstractInputSystem.Instance.PlayerInput.JumpObservable
            .Where(_ => m_CanJump)
            .Subscribe(_ => {
                MoveSwitch = 0;
                m_JumpTimer = 0.0f;
                m_IsJump = true;
                m_CharacterAnimator.Jump(true);
            }).AddTo(this);
    }

    private void LookGoal()
    {
        this.UpdateAsObservable()
            .First(_ => m_Goal.IsGoal)
            .Subscribe(_ => {
                transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -1));
            }).AddTo(this);
    }
}

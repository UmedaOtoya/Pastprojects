using UnityEngine;
using UniRx;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField, Header("オフセット")] private Vector3 m_Offset = Vector3.zero;
    [SerializeField, Header("カメラ公転速度")] private float m_RotateSpeed = 90f;
    [SerializeField, Header("カメラ回り込み速度")] private float m_GetBehindSpeed = 0.3f;
    [SerializeField, Header("X軸角度")] private float m_PolarAngle = 45.0f;
    [SerializeField, Header("最大X軸角度")] private float m_MaxPolarAngle = 75.0f;
    [SerializeField, Header("最小X軸角度")] private float m_MinPolarAngle = 5.0f;
    [SerializeField, Header("Y軸角度")] private float m_AzimuthalAngle = 0.0f;
    [SerializeField, Header("カメラ距離移動量")] private float m_DistanceValue = 3.0f;
    [SerializeField, Header("カメラ最近距離")] private float m_MinDistance = 1.0f;
    [SerializeField, Header("カメラ距離段階"),Min(1)] private uint m_DistanceStep = 3;

    private float m_Distance = 0.0f;
    private GameObject m_Target = default;
    private PlayerMovementTest m_PlayerFlag = default;
    private Vector2 m_InputVelocity = Vector2.zero;

    private void Start()
    {
        m_Target = GameObject.Find("Player");
        m_PlayerFlag = m_Target.GetComponent<PlayerMovementTest>();
        m_Distance = m_MinDistance + m_DistanceValue;
        // カメラの移動入力取得
        AbstractInputSystem.Instance.UserInput.CameraObservable.Subscribe(v => m_InputVelocity = v).AddTo(this);
        updateDistance();
        LookPlayerForward();
    }

    private void LateUpdate()
    {
        if (m_PlayerFlag.IsDead) return;
        updateAngle(m_InputVelocity.x, m_InputVelocity.y);
        var lookAtPos = m_Target.transform.position + m_Offset;
        updatePosition(lookAtPos);
        transform.LookAt(lookAtPos);
    }

    private void updateAngle(float x, float y)
    {
        m_AzimuthalAngle -= x * m_RotateSpeed * Time.deltaTime;
        m_AzimuthalAngle = Mathf.Repeat(m_AzimuthalAngle, 360);

        m_PolarAngle += y * m_RotateSpeed * Time.deltaTime;
        m_PolarAngle = Mathf.Clamp(m_PolarAngle, m_MinPolarAngle, m_MaxPolarAngle);
    }

    private void updatePosition(Vector3 lookAtPos)
    {
        var da = -(m_AzimuthalAngle + 90.0f) * Mathf.Deg2Rad;
        var dp = m_PolarAngle * Mathf.Deg2Rad;
        transform.position = new Vector3(
            lookAtPos.x + m_Distance * Mathf.Sin(dp) * Mathf.Cos(da),
            lookAtPos.y + m_Distance * Mathf.Cos(dp),
            lookAtPos.z + m_Distance * Mathf.Sin(dp) * Mathf.Sin(da));
    }

    private void updateDistance()
    {
        // カメラの距離移動
        AbstractInputSystem.Instance.UserInput.L1Observable.Subscribe(_ => {
            m_Distance += m_DistanceValue;
            if(m_Distance > m_DistanceValue * (m_DistanceStep - 1) + m_MinDistance)
                m_Distance = m_MinDistance;
        }).AddTo(this);

    }

    private void LookPlayerForward()
    {
        AbstractInputSystem.Instance.UserInput.R1Observable.Subscribe(_ => {
            DOTween.To(() => m_AzimuthalAngle,         //動かしたい値
                value => m_AzimuthalAngle = value,       //値の更新
                m_Target.transform.eulerAngles.y,           //値の最大値
                m_GetBehindSpeed);                              //最大値までの秒数
        }).AddTo(this);
    }
}

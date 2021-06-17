using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletBase))]
public class Bullet : MonoBehaviour
{
    [SerializeField, Tooltip("弾の根元位置")]
    private Transform m_Root = null;
    [SerializeField, Tooltip("弾の先端位置")]
    private Transform m_Tip = null;
    [SerializeField, Tooltip("弾の残る時間")]
    private float m_MaxLifeTime = 5f;
    [SerializeField, Tooltip("衝突時のエフェクト")]
    private GameObject m_InpactEfect = default;
    [SerializeField, Tooltip("衝突時のエフェクトの残る時間")]
    private float m_InpactEfectLifeTime = 5f;
    [SerializeField, Tooltip("衝突時のエフェクトのオフセット")]
    private float m_ImpactEfectSpawnOffset = 0.1f;
    [SerializeField, Tooltip("弾の速度")]
    private float m_Speed = 20.0f;
    [SerializeField, Tooltip("重力の影響力")]
    private float m_GravityDownAcceleration = 0.0f;
    [SerializeField, Tooltip("弾道補正距離")]
    private float m_TrajectoryCorrectionDistance = -1.0f;

    private BulletBase m_BulletBase = null;
    private Vector3 m_LastRootPosition = Vector3.zero;
    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 m_TrajectoryCorrectionVector = Vector3.zero;
    private Vector3 m_ConsumedTrajectoryCorrectionVector = Vector3.zero;
    private bool m_HasTrajectoryOverride = false;
    private Rigidbody m_Rigidbody = default;
    private AttackSender m_Sender = default;

    private void OnEnable()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Sender = GetComponent<AttackSender>();
        m_BulletBase = GetComponent<BulletBase>();
        m_BulletBase.m_OnShoot += OnShoot;

        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnShoot()
    {
        m_Sender.DamageCoefficient = m_BulletBase.DamageCoefficient;
        m_LastRootPosition = m_Root.position;
        m_Velocity = transform.forward * m_Speed;
        m_Rigidbody.position += m_BulletBase.inheritedMuzzleVelocity * Time.deltaTime;

        m_HasTrajectoryOverride = true;
        Vector3 cameraToMuzzle = (m_BulletBase.initialPosition - m_BulletBase.cameraInfo.transform.position);
        m_TrajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle, m_BulletBase.cameraInfo.transform.forward);
        if(m_TrajectoryCorrectionDistance == 0) {
            transform.position += m_TrajectoryCorrectionVector;
            m_ConsumedTrajectoryCorrectionVector = m_TrajectoryCorrectionVector;
        }
        else if(m_TrajectoryCorrectionDistance < 0) {
            m_HasTrajectoryOverride = false;
        }
    }

    private void Update()
    {
        //移動
        m_Rigidbody.position += m_Velocity * Time.deltaTime;
        m_Rigidbody.position += m_BulletBase.inheritedMuzzleVelocity * Time.deltaTime;
       //弾道の補正
        if (m_HasTrajectoryOverride && m_ConsumedTrajectoryCorrectionVector.sqrMagnitude < m_TrajectoryCorrectionVector.sqrMagnitude) {

            Vector3 correctionVector = m_TrajectoryCorrectionVector - m_ConsumedTrajectoryCorrectionVector;
            float distanceFrame = (m_Root.position - m_LastRootPosition).magnitude;
            Vector3 correctionFrame = (distanceFrame / m_TrajectoryCorrectionDistance) * m_TrajectoryCorrectionVector;
            correctionFrame = Vector3.ClampMagnitude(correctionFrame, correctionVector.magnitude);
            m_ConsumedTrajectoryCorrectionVector += correctionFrame;

            //補正の終了
            if(m_ConsumedTrajectoryCorrectionVector.sqrMagnitude == m_TrajectoryCorrectionVector.sqrMagnitude) {
                m_HasTrajectoryOverride = false;
            }

            transform.position += correctionFrame;
        }

        transform.forward = m_Velocity.normalized;

        //重力
        if(m_GravityDownAcceleration > 0) {
            m_Velocity += Vector3.down * m_GravityDownAcceleration * Time.deltaTime;
        }

        m_LastRootPosition = m_Root.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Onhit(collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void Onhit(Vector3 point, Vector3 normal)
    {
        if (m_InpactEfect != null) {
            GameObject inpactEfectInstance = Instantiate(m_InpactEfect, point + (normal * m_ImpactEfectSpawnOffset), Quaternion.LookRotation(normal));
            if (m_InpactEfectLifeTime > 0) {
                Destroy(inpactEfectInstance.gameObject, m_InpactEfectLifeTime);
            }
        }

        Destroy(this.gameObject);
    }
}

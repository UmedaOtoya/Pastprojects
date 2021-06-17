using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : AbstractBaseWeapon
{
    [SerializeField, Tooltip("UIに表示する際の武器の名前")]
    protected string m_WeaponName = ("NONAME");

    [Header("内部参照")]
    [SerializeField, Tooltip("銃口のポジション")]
    protected Transform m_WeaponMuzzle = default;

    [Header("シュートパラメータ")]
    [SerializeField, Tooltip("プレイヤー弾プレハブ")]
    protected BulletBase m_PlayerBulletPrefab = default;
    [SerializeField, Tooltip("敵弾プレハブ")]
    protected BulletBase m_EnemyBulletPrefab = default;
    [SerializeField, Tooltip("次のショットまでの時間")]
    protected float m_DelayBetweenShots = 0.5f;
    [SerializeField, Tooltip("弾道のブレ")]
    protected float m_BulletSpreadAngle = 0f;
    [SerializeField, Tooltip("1 ショットあたりの弾丸の量")]
    protected int m_BulletsPerShot = 1;

    [Header("Audio & Visual")]
    [SerializeField, Tooltip("ショット時のSE")]
    protected string m_ShotSE = ("");
    [SerializeField, Tooltip("ショット時の銃口の発光")]
    protected GameObject m_MuzzleFlashPrefab = default;
    [SerializeField, Tooltip("銃口エフェクトの位置調整")]
    protected Vector3 m_MuzzleFlashOffset = Vector3.zero;

    protected Vector3 m_LastMuzzlePosition = Vector3.zero;
    protected float m_LastTimeShot = Mathf.NegativeInfinity;
    private Weapon m_Weapon = default;

    public Vector3 MuzzleWorldVelocity { get; private set; }
    public float DamageCoefficient { get; private set; } = 1f;
    public Camera WeaponCamera { get { return Camera.main; } }
    public string WeaponName { get { return m_WeaponName; } }

    private void Awake()
    {
        m_Weapon = GetComponent<Weapon>();
        m_LastMuzzlePosition = m_WeaponMuzzle.position;
        m_CurrentAmmo = m_MaxAmmo;
    }

    private void Update()
    {
        MuzzleWorldVelocity = (m_WeaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
        m_LastMuzzlePosition = m_WeaponMuzzle.position;
    }

    public override bool Shoot() => TryShoot();

    public override bool Release() => HandleRelease();

    protected virtual bool TryShoot()
    {
        if (m_LastTimeShot + m_DelayBetweenShots < Time.time && CurrentAmmo > 0) {
            HandleShoot();
            return true;
        }

        return false;
    }

    protected virtual void HandleShoot()
    {
        var soundHandle = SoundPlayer.Instance.PlaySE(m_ShotSE, transform.position);

        DamageCoefficient = m_Weapon.DamageCoefficient;
        if (transform.root.CompareTag("Player")) {
            m_CurrentAmmo--;
            BulletInstance(m_PlayerBulletPrefab);
        }
        else {
            BulletInstance(m_EnemyBulletPrefab); 
        }

        if (m_MuzzleFlashPrefab != null) {
            GameObject muzzleFlashInstance = Instantiate(m_MuzzleFlashPrefab, m_WeaponMuzzle.position + m_MuzzleFlashOffset, m_WeaponMuzzle.rotation, m_WeaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2f);
        }

        m_LastTimeShot = Time.time;

        if (m_CurrentAmmo <= 0) {
            m_Weapon.PurgeMyselfIfNeeded();
            Destroy(gameObject);
        }
    }
    
    protected virtual bool HandleRelease() => false;

    /// <summary>
    /// 弾道をランダムにブレさせる
    /// </summary>
    /// <param name="shootTransform"></param>
    /// <returns></returns>
    protected Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = m_BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);

        return spreadWorldDirection;
    }

    protected void BulletInstance(BulletBase Bullet)
    {
        for (int i = 0; i < m_BulletsPerShot; i++) {
            Vector3 shotDirection = GetShotDirectionWithinSpread(m_WeaponMuzzle);
            BulletBase newBullet = Instantiate(Bullet, m_WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
            newBullet.Shoot(this);
        }
    }
}

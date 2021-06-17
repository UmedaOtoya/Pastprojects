using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShot : GunBase
{
    [SerializeField, Tooltip("チャージ中のエフェクト")]
    private GameObject m_ChargeEfect = null;
    private GameObject m_ChargeEfectInstance = null;
    private bool m_IsCharging = false;
    private ISoundHandle soundHandle = null;

    private void Update()
    {
        if (soundHandle == null) { return; }
        soundHandle.Position = transform.position;
    }

    protected override bool TryShoot()
    {
        if (m_IsCharging) {
            if (m_LastTimeShot + m_DelayBetweenShots < Time.time && CurrentAmmo > 0) {
                m_IsCharging = false;
                Destroy(m_ChargeEfectInstance);
                soundHandle?.Stop();
                soundHandle = SoundPlayer.Instance.PlaySE(m_ShotSE, transform.position);
                HandleShoot();
                return true;
            }
        }
        else {
            m_LastTimeShot = Time.time;
            m_IsCharging = true;
            soundHandle?.Stop();
            soundHandle = SoundPlayer.Instance.PlaySE("se_charge", transform.position);

            if (m_ChargeEfect != null) {
                m_ChargeEfectInstance = Instantiate(m_ChargeEfect, m_WeaponMuzzle.position, m_WeaponMuzzle.rotation, m_WeaponMuzzle.transform);
            }
            soundHandle.IsLoop = true;
            return false;
        }
        return false;
    }

    protected override bool HandleRelease()
    {
        Destroy(m_ChargeEfectInstance);
        soundHandle.Stop();
        soundHandle = null;
        m_LastTimeShot = Time.time;
        m_IsCharging = false;
        return true;
    }

    private void OnDestroy()
    {
        soundHandle?.Stop();
        soundHandle = null;
    }
}

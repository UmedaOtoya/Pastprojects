using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : GunBase
{
    protected override void HandleShoot()
    {
        SoundPlayer.Instance.PlaySE(m_ShotSE, transform.position);

        if (transform.root.CompareTag("Player")) {
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
    }
}

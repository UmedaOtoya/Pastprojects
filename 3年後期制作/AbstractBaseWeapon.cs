using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBaseWeapon : MonoBehaviour
{
    [SerializeField, Tooltip("UIに表示する際の武器のアイコン")]
    protected Sprite m_WeaponIcon = default;
    [SerializeField, Tooltip("弾薬数")]
    protected int m_MaxAmmo = 10;
    protected int m_CurrentAmmo = 0;

    public Sprite WeaponIcon { get { return m_WeaponIcon; } }
    public int CurrentAmmo { get { return m_CurrentAmmo; } }
    public int MaxAmmo { get { return m_MaxAmmo; } }

    public virtual bool Shoot() { return false; }

    public virtual bool Release() { return false; }

    public virtual bool SowrdOn() { return false; }
    public virtual bool SowrdOff() { return false; }

}

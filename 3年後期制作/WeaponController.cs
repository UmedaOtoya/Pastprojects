using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private AbstractBaseWeapon m_ThisWeapon = default;

    public bool Shoot() => m_ThisWeapon.Shoot();

    public bool Release() => m_ThisWeapon.Release();

    public bool SowrdOn() => m_ThisWeapon.SowrdOn();

    public bool SowrdOff() => m_ThisWeapon.SowrdOff();

}

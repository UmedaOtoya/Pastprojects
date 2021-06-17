using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletBase : MonoBehaviour
{
    public Camera cameraInfo { get; private set; }
    public Vector3 initialPosition { get; private set; }
    public Vector3 initialDirection { get; private set; }
    public Vector3 inheritedMuzzleVelocity { get; private set; }
    public float DamageCoefficient { get; private set; } = 1f;

    public UnityAction m_OnShoot;

    public void Shoot(GunBase controller)
    {
        cameraInfo = controller.WeaponCamera;
        initialPosition = transform.position;
        initialDirection = transform.forward;
        inheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
        DamageCoefficient = controller.DamageCoefficient;
        
        if (m_OnShoot != null) {
            m_OnShoot.Invoke();
        }
    }
}

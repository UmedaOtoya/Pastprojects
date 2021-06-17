using System;
using UnityEngine;
using UniRx;

public class Mebius : MonoBehaviour
{
    [SerializeField, Tooltip(" ループの範囲")]
    private LoopRange loopRange = default;
    [SerializeField, Tooltip("波紋エフェクト")]
    private GameObject ripple = default;
    private Bounds bounds = default;


    private ReactiveProperty<bool> IsWarp = new ReactiveProperty<bool>(false);

    public IObservable<bool> Observable => IsWarp;

    private void WarpTrigger()
    {
        IsWarp.Value = true;
        IsWarp.Value = false;
    }

    private void Awake()
    {
        bounds = loopRange.Bounds;
    }

    private void Update()
    {
        // 範囲から飛び出したら、反対側にワープ
        if (transform.position.x < bounds.min.x)
        {
            Instantiate(ripple, transform.position, Quaternion.Euler(0, 90, 0));
            WarpTrigger();
            Vector3 pos = transform.position;
            pos.x += bounds.size.x;
            transform.position = pos;
        }
        if (transform.position.x > bounds.max.x)
        {
            Instantiate(ripple, transform.position, Quaternion.Euler(0, 90, 0));
            WarpTrigger();
            Vector3 pos = transform.position;
            pos.x -= bounds.size.x;
            transform.position = pos;
        }
        if (transform.position.y < bounds.min.y)
        {
            Instantiate(ripple, transform.position, Quaternion.Euler(90, 0, 0));
            WarpTrigger();
            Vector3 pos = transform.position;
            pos.y += bounds.size.y;
            transform.position = pos;
        }
        if (transform.position.y > bounds.max.y)
        {
            Instantiate(ripple, transform.position, Quaternion.Euler(90, 0, 0));
            WarpTrigger();
            Vector3 pos = transform.position;
            pos.y -= bounds.size.y;
            transform.position = pos;
        }
        if (transform.position.z < bounds.min.z)
        {
            Instantiate(ripple, transform.position, Quaternion.Euler(0, 0, 0));
            WarpTrigger();
            Vector3 pos = transform.position;
            pos.z += bounds.size.z;
            transform.position = pos;
        }
        if (transform.position.z > bounds.max.z)
        {
            Instantiate(ripple, transform.position, Quaternion.Euler(0, 0, 0));
            WarpTrigger();
            Vector3 pos = transform.position;
            pos.z -= bounds.size.z;
            transform.position = pos;
        }
    }
}

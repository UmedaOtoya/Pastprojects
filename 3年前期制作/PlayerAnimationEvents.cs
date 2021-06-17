using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private bool m_IsPushMotion = false;
    private bool m_IsAttachMotion = false;

    /// <summary> プレイヤーがアニメーション中か </summary>
    public bool IsPlayerMotion { get { return m_IsPushMotion || m_IsAttachMotion; } }
    /// <summary>死亡アニメーションが終わっているか </summary>
    public bool IsDeadMotioinEnd { get; private set; }

    private void Start()
    {
        m_IsPushMotion = false;
        m_IsAttachMotion = false;
        IsDeadMotioinEnd = false;
    }

    private void StartPush()
    {
        m_IsPushMotion = true;
        m_IsAttachMotion = false;
    }
    private void EndPush() => m_IsPushMotion = false;

    private void StartAttach()
    {
        m_IsAttachMotion = true;
        m_IsPushMotion = false;
    }

    private void EndAttach() => m_IsAttachMotion = false;

    private void EndDead() => IsDeadMotioinEnd = true;

    private void DeadExplosion(GameObject efect) => Instantiate(efect, transform);
}

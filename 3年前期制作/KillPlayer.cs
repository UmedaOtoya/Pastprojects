using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private GameObject m_Player = default;
    private PlayerMovementTest m_PlayerFlag = default;
    private RRCharacterAnimator m_PlayerAnimator = default;
    void Start()
    {
        m_Player = GameObject.Find("Player");
        if(m_Player != null)
        {
            m_PlayerFlag = m_Player.GetComponent<PlayerMovementTest>();
            m_PlayerAnimator = m_Player.GetComponentInChildren<RRCharacterAnimator>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject.CompareTag("Needle"))  SoundManager.Instance.Play(SoundType.NeedleDamage, transform);
            else                                                    SoundManager.Instance.Play(SoundType.PlayerDead, other.transform.position);

            m_PlayerFlag.IsDead = true;
            m_PlayerAnimator.Die();
        }
    }
}

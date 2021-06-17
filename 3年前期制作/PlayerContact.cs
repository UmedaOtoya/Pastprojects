using UnityEngine;

public class PlayerContact : MonoBehaviour
{
    private RRCharacterAnimator m_CharacterAnimator = null;
    private PlayerMovementTest m_Player = default;

    private void Start()
    {
        m_CharacterAnimator = gameObject.GetComponentInChildren<RRCharacterAnimator>();
        m_Player = gameObject.GetComponent<PlayerMovementTest>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            m_Player.IsDead = true;
            m_CharacterAnimator.Die();
            SoundManager.Instance.Play(SoundType.PlayerDead, transform);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!m_Player.OnGround && collision.collider.tag.Contains("Block"))
            m_Player.MoveSwitch = 0;
        else
            m_Player.MoveSwitch = 1;
    }

    private void OnCollisionExit(Collision collision)
    {
        m_Player.MoveSwitch = 1;
    }
}

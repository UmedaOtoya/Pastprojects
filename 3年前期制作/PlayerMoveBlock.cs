using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerMoveBlock : MonoBehaviour
{
    [SerializeField] private Animator m_PlayerAnimator = default;
    [SerializeField] private PlayerAnimationEvents m_AnimationEvent = default;

    private PlayerMovementTest m_Player = default;
    private InstantiateHinge m_Hinge = default;
    private PlayerCenterRay m_Ray = default;
    private HingeView m_SelectedHinge = default;

    private void Start()
    {
        m_Player = gameObject.GetComponentInParent<PlayerMovementTest>();
        m_Hinge = gameObject.GetComponent<InstantiateHinge>();
        m_Ray = gameObject.GetComponent<PlayerCenterRay>();

        AbstractInputSystem.Instance.PlayerInput.PushObservable.
            Where(_ => CanPush()).
            Subscribe(i => {
                m_PlayerAnimator.SetTrigger("Push");
                var pushBlock = m_Ray.LeftHit.collider.GetComponentInParent<IBlockPush>();
                pushBlock.Push(transform.parent);
            }).AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hinge"))
        {
            m_SelectedHinge = other.gameObject.GetComponentInParent<HingeView>();
        }
    }

    /// <summary>
    /// ブロックが押せるか
    /// </summary>
    /// <returns></returns>
    private bool CanPush()
    {
        //2本のレイのどちらかがブロックに当たっていない場合＋プレイヤーがモーション中の場合false
        if (m_AnimationEvent.IsPlayerMotion || !m_Player.OnGround || m_Player.IsDead ||
            m_Ray.LeftHit.collider == null || m_Ray.RightHit.collider == null || 
            !(m_Ray.LeftHit.collider.CompareTag("PushBlock") || m_Ray.LeftHit.collider.CompareTag("MoveBlock")) ||
            !(m_Ray.RightHit.collider.CompareTag("PushBlock") || m_Ray.RightHit.collider.CompareTag("MoveBlock")))
            return false;

        //ブロックが動いていた場合False
        var blockStatus = m_Ray.LeftHit.collider.GetComponentInParent<IBlockStatus>();
        if (blockStatus.IsShifting || blockStatus.IsFalling)
            return false;

        //2本のレイが同じブロックに当たっていた場合True
        if (m_Ray.LeftHit.collider == m_Ray.RightHit.collider)
            return true;

        //蝶番で結合されていた場合、それぞれのブロックが同じ塊の内ならTrue
        if (m_Hinge.InHinge)
        {
            List<KeyValuePair<HingeView, HingeView>> hinges = new List<KeyValuePair<HingeView, HingeView>>();
            List<BlockView> blocks = new List<BlockView>();
            List<HingeView> rotates = new List<HingeView>();
            m_SelectedHinge.GetAllProssesionObjects(hinges, blocks, rotates);

            bool isBlock1 = false;
            bool isBlock2 = false;

            var block1 = m_Ray.LeftHit.collider.GetComponentInParent<BlockView>();
            var block2 = m_Ray.RightHit.collider.GetComponentInParent<BlockView>();

            foreach (var block in blocks)
            {
                if (block == block1) isBlock1 = true;
                else if (block == block2) isBlock2 = true;
            }

            return isBlock1 && isBlock2 && !m_AnimationEvent.IsPlayerMotion;
        }

        return false;
    }
}

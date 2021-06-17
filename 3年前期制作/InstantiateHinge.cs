using UnityEngine;
using UniRx;

public class InstantiateHinge : MonoBehaviour
{
    [SerializeField] private Animator m_PlayerAnimator = default;
    [SerializeField] private PlayerAnimationEvents m_AnimationEvent = default;
    [SerializeField, Header("蝶番プレファブ")] private GameObject m_Hinge = default;

    private GameObject m_GimmickParent = default;
    private HingeUI m_HingeUI = default;
    private PlayerCenterRay m_Ray = default;
    private HingeView m_SelectedHinge = default;
    private bool m_CanAttach = false;
    private bool m_CanRemove = false;
    private Vector3 m_HingePosition = Vector3.zero;
    private Quaternion m_HingeRotation = Quaternion.identity;

    public bool InHinge { get; private set; }

    private void Start()
    {
        m_GimmickParent = GameObject.Find("GimmickParent");
        m_HingeUI = GameObject.Find("HingeUI").GetComponent<HingeUI>();
        m_Ray = gameObject.GetComponent<PlayerCenterRay>();
        m_CanAttach = false;
        m_CanRemove = false;
        InHinge = false;
        Attach();
        Remove();
    }
    private void Update()
    {
        bool blockOnBlock = false;
        bool IsMoving = false;

        if (m_Ray.LeftHit.collider != null && m_Ray.RightHit.collider != null && m_Ray.LeftHit.collider.tag.Contains("Block") && m_Ray.RightHit.collider.tag.Contains("Block"))
        {
            float hitBlockDistance = Vector3.Distance(m_Ray.LeftHit.transform.position, m_Ray.RightHit.transform.position);
            var block1 = m_Ray.LeftHit.collider.GetComponentInParent<BlockView>();
            var block2 = m_Ray.RightHit.collider.GetComponentInParent<BlockView>();
            blockOnBlock = (hitBlockDistance > 0.9f) && (hitBlockDistance < 1.1f);
            IsMoving = block1.IsRotate || block2.IsRotate;
        }
        else
        {
            blockOnBlock = false;
            IsMoving = true;
        }

        m_CanAttach = blockOnBlock && !InHinge && !m_AnimationEvent.IsPlayerMotion && m_HingeUI.RemainingHinge > 0;
        m_CanRemove = !IsMoving && InHinge && !m_AnimationEvent.IsPlayerMotion && m_SelectedHinge != null;

        Debug.Log($"InHinge:{InHinge}");
        Debug.Log($"m_CanAttach:{m_CanAttach}");
        Debug.Log($"m_CanRemove:{m_CanRemove}");

        if (m_CanAttach)
        {
            UpdateHingePosition();
            UpdateHingeRotation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hinge"))
        {
            InHinge = true;
            m_SelectedHinge = other.gameObject.GetComponentInParent<HingeView>();
        }
        if (other.CompareTag("AbsLockHinge")) InHinge = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hinge") || other.CompareTag("AbsLockHinge"))  InHinge = false;
    }

    private void Attach()
    {
        AbstractInputSystem.Instance.PlayerInput.AttachHingeObservable.
            Where(_ => m_CanAttach).
            Subscribe(_ => {
                m_HingeUI.RemainingHinge--;
                m_PlayerAnimator.SetTrigger("Attach");
                SoundManager.Instance.Play(SoundType.HIngeInstance, transform);
                Instantiate(m_Hinge, m_HingePosition, m_HingeRotation, m_GimmickParent.transform);
            }).AddTo(this);
    }

    private void Remove()
    {
        AbstractInputSystem.Instance.PlayerInput.AttachHingeObservable.
            Where(_ => m_CanRemove).
            Subscribe(_ => {
                m_HingeUI.RemainingHinge++;
                m_PlayerAnimator.SetTrigger("Attach");
                m_SelectedHinge.Destroy();
                InHinge = false;
            }).AddTo(this);
    }

    private Vector3 UpdateHingePosition()
    {
        Vector3 HitInequality = m_Ray.LeftHit.transform.position - m_Ray.RightHit.transform.position;
        if (HitInequality.x <= -0.9f && HitInequality.x >= -1.1f)
            return m_HingePosition = m_Ray.LeftHit.transform.position + new Vector3(0.5f, 0.0f, -0.5f);
        else if (HitInequality.x <= 1.1f && HitInequality.x >= 0.9f)
            return m_HingePosition = m_Ray.LeftHit.transform.position + new Vector3(-0.5f, 0.0f, 0.5f);
        else if (HitInequality.z <= -0.9f && HitInequality.z >= -1.1f)
            return m_HingePosition = m_Ray.LeftHit.transform.position + new Vector3(0.5f, 0.0f, 0.5f);
        else if (HitInequality.z <= 1.1f && HitInequality.z >= 0.9f)
            return m_HingePosition = m_Ray.LeftHit.transform.position + new Vector3(-0.5f, 0.0f, -0.5f);

        return m_HingePosition = Vector3.zero;
    }

    private Quaternion UpdateHingeRotation()
    {
        Vector3 vec1 = -(m_Ray.LeftHit.transform.position - m_HingePosition).normalized;
        Vector3 vec2 = -(m_Ray.RightHit.transform.position - m_HingePosition).normalized;
        return m_HingeRotation = Quaternion.LookRotation(vec1 + vec2);
    }
}

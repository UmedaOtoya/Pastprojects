using UnityEngine;

public class PlayerCenterRay : MonoBehaviour
{
    [SerializeField, Header("レイの長さ"), Range(0.0f, 0.7f)] private float m_RayLength = 0.7f;
    [SerializeField, Header("2本のレイの幅"), Range(0.27f, 1.0f)] private float m_RayWide = 0.27f;
    [SerializeField, Header("2本のレイの拡がり"), Range(0.0f, 1.0f)] private float m_RayDirectionBroaden = 1.0f;

    private RaycastHit m_Hit1 = default;
    private RaycastHit m_Hit2 = default;

    public RaycastHit LeftHit { get { return m_Hit1; }}
    public RaycastHit RightHit { get { return m_Hit2; } }

    private void Update()
    {
        Ray leftRay = new Ray(transform.position - transform.right * m_RayWide / 2, transform.forward - transform.right * m_RayDirectionBroaden);
        Ray rightRay = new Ray(transform.position + transform.right * m_RayWide / 2, transform.forward + transform.right * m_RayDirectionBroaden);
        Physics.Raycast(leftRay, out m_Hit1, m_RayLength);
        Physics.Raycast(rightRay, out m_Hit2, m_RayLength);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position - transform.right * m_RayWide / 2, (transform.forward - transform.right * m_RayDirectionBroaden).normalized * m_RayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + transform.right * m_RayWide / 2, (transform.forward + transform.right * m_RayDirectionBroaden).normalized * m_RayLength);
    }
}

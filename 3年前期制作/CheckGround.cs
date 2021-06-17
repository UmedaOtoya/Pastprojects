using UnityEngine;
using UnityEngine.Events;

public class CheckGround : MonoBehaviour
{
    [SerializeField, Header("接地した場合の処理")]
    private UnityEvent OnEnterGround = default;
    [SerializeField, Header("地面から離れた場合の処理")]
    private UnityEvent OnExitGround = default;
    //接地数
    private int enterNum = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Block") || other.CompareTag("Ground"))
        {
            enterNum++;
            OnEnterGround.Invoke();
            SoundManager.Instance.Play(SoundType.PlayerWalk, transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Block") || other.CompareTag("Ground"))
        {
            enterNum--;
            if (enterNum <= 0) 
                OnExitGround.Invoke();
        }
    }
}

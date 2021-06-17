using UnityEngine;
using Cinemachine;

public class MebiusCameraController : MonoBehaviour
{
    public Vector3 offset;
    Transform mainCameraTransform;

    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        CinemachineCore.CameraUpdatedEvent.AddListener(UpdateCamera);
    }

    private void OnEnable() => CinemachineCore.CameraUpdatedEvent.AddListener(UpdateCamera);

    private void OnDisable() => CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateCamera);

    private void OnDestroy() => CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateCamera);

    /// <summary>
    /// 全てのカメラの向きをCinemachineのMainCameraと同期させる
    /// </summary>
    /// <param name="brain"></param>
    private void UpdateCamera(CinemachineBrain brain)
    {
        transform.position = mainCameraTransform.position + offset;

        foreach (Transform child in transform)
        {
            child.rotation = mainCameraTransform.rotation;
        }
    }
}

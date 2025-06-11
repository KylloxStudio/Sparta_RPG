using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera MainCamera { get; private set; }
    public CinemachineVirtualCamera VirtualCamera { get; private set; }

    private void Awake()
    {
        MainCamera = Camera.main;
        VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        VirtualCamera.Follow = Singleton<InGameManager>.Instance().LocalPlayer.CameraLookPoint;
        VirtualCamera.LookAt = Singleton<InGameManager>.Instance().LocalPlayer.CameraLookPoint;
    }

    public void ShakeCamera(float intensity, float time)
    {
        StartCoroutine(ProcessShakeCamera(intensity, time));
    }

    private IEnumerator ProcessShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin multiChannelPerlin = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(time);

        multiChannelPerlin.m_AmplitudeGain = 0f;

        yield break;
    }

    public void SetCameraLookRotation(Quaternion rot)
    {
        Quaternion curRot = Singleton<InGameManager>.Instance().LocalPlayer.CameraLookPoint.transform.rotation;
        Singleton<InGameManager>.Instance().LocalPlayer.CameraLookPoint.transform.rotation = Quaternion.Slerp(curRot, rot, Time.deltaTime * 5f);
    }
}

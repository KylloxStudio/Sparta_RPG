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
        VirtualCamera.Follow = Singleton<Player>.Instance().CameraLookPoint;
        VirtualCamera.LookAt = Singleton<Player>.Instance().CameraLookPoint;
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
}

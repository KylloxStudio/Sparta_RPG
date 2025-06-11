using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitCode
{
    Mika
}

public class Player : MonoBehaviour
{
    [SerializeField] private UnitCode _unitCode;
    public UnitCode UnitCode => _unitCode;

    [SerializeField] private PlayerStatsData _statsData;

    [SerializeField] private Transform _cameraLookPoint;
    public Transform CameraLookPoint => _cameraLookPoint;

    public PlayerStats Stats { get; private set; }
    public PlayerAIController Controller { get; private set; }
    public PlayerAnimation Animation { get; private set; }
    public Weapon Weapon { get; private set; }

    private void Awake()
    {
        if (_statsData == null)
        {
            Debug.LogError("player stats data is null.");
            return;
        }

        Stats = new PlayerStats(_statsData);
        Controller = GetComponent<PlayerAIController>();
        Animation = GetComponentInChildren<PlayerAnimation>();
        Weapon = GetComponentInChildren<Weapon>();
    }

    private void Start()
    {
        Singleton<InGameManager>.Instance().SetLocalPlayer(this);
    }
}

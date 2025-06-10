using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private PlayerStatsData _statsData;
    [SerializeField] private Transform _cameraLookPoint;
    public Transform CameraLookPoint => _cameraLookPoint;

    public PlayerStats Stats { get; private set; }
    public PlayerController Controller { get; private set; }
    public Weapon Weapon { get; private set; }

    protected override void OnAwake()
    {
        if (_statsData == null)
        {
            Debug.LogError("player stats data is null.");
            return;
        }

        Stats = new PlayerStats(_statsData);
        Controller = GetComponent<PlayerController>();
        Weapon = GetComponentInChildren<Weapon>();
    }
}

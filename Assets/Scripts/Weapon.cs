using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { };

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfoData _infoData;
    public WeaponInfo Info { get; private set; }

    public AmmoEvent OnAmmoEvent = new AmmoEvent();

    private void Awake()
    {
        if (_infoData == null)
        {
            Debug.LogError("weapon info data is null.");
            return;
        }

        Info = new WeaponInfo(_infoData);
    }
}
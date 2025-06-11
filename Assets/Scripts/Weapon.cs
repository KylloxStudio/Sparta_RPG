using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { };

public class Weapon : MonoBehaviour
{
    private Player _player;

    [SerializeField] private WeaponInfoData _infoData;
    public WeaponInfo Info { get; private set; }

    [SerializeField] private Transform _checkAttackPoint;
    public Transform CheckAttackPoint => _checkAttackPoint;

    [SerializeField] private Transform _bulletSpawnPoint;
    public Transform BulletSpawnPoint => _bulletSpawnPoint;

    public AmmoEvent OnAmmoEvent = new AmmoEvent();

    public GameObject Victim { get; set; }

    [SerializeField] private AudioSource _shootSound;
    [SerializeField] private AudioSource _reloadSound;

    private void Awake()
    {
        if (_infoData == null)
        {
            Debug.LogError("weapon info data is null.");
            return;
        }

        Info = new WeaponInfo(_infoData);
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (!_player.Controller.IsReloading)
        {
            if (Info.CurAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && Info.CurAmmo < Info.MaxAmmo && !_player.Controller.IsUsingSpecialSkill && !_player.Controller.IsUsingExSkill))
            {
                Reload();
            }
        }
    }

    public void Shoot()
    {
        if (Info.CurAmmo <= 0)
        {
            return;
        }

        switch (Info.Type)
        {
            case WeaponType.Rifle or WeaponType.Sniper:
                {
                    Ray ray;
                    if (_player.Controller.Target != null)
                    {
                        ray = new Ray(_bulletSpawnPoint.transform.position, _player.Controller.Target.transform.position - transform.position);
                    }
                    else
                    {
                        ray = new Ray(_bulletSpawnPoint.transform.position, _player.transform.forward);
                    }

                    Bullet bullet = Singleton<ObjectPoolManager>.Instance().Get("Bullet").GetComponent<Bullet>();
                    bullet.transform.SetPositionAndRotation(_bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
                    bullet.Initialize(_player.gameObject, ray, LayerMask.GetMask("Enemy", "Structure"), Info.AttackDamage, ignoreDefense: _player.UnitCode == UnitCode.Mika);

                    GameObject muzzleFlash = Singleton<ObjectPoolManager>.Instance().Get("MuzzleFlash");
                    muzzleFlash.transform.SetPositionAndRotation(_bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
                    //muzzleFlash.transform.SetParent(_bulletSpawnPoint);
                    break;
                }
            case WeaponType.Shotgun:
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Ray ray = new Ray(_bulletSpawnPoint.transform.position, _player.transform.forward + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));

                        Bullet bullet = Singleton<ObjectPoolManager>.Instance().Get("Bullet").GetComponent<Bullet>();
                        bullet.transform.SetPositionAndRotation(_bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
                        bullet.Initialize(_player.gameObject, ray, LayerMask.GetMask("Enemy", "Structure"), Info.AttackDamage / 12f, ignoreDefense: _player.UnitCode == UnitCode.Mika);

                        GameObject muzzleFlash = Singleton<ObjectPoolManager>.Instance().Get("MuzzleFlash");
                        muzzleFlash.transform.SetPositionAndRotation(_bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
                    }

                }
                break;
            default:
                Debug.LogError("Cannot Found Weapon Type: " + Info.Type);
                break;
        }

        _shootSound.Play();
        Singleton<CameraManager>.Instance().ShakeCamera(Info.AttackDamage * 0.001f, 0.08f);

        Info.CurAmmo--;
        OnAmmoEvent?.Invoke(Info.CurAmmo, Info.MaxAmmo);
    }

    private void Reload()
    {
        _reloadSound.Play();
        _player.Controller.EventReload();
    }
}
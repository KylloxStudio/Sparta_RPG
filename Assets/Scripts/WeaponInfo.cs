using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public enum WeaponType
{
    Rifle,
    Shotgun,
    Sniper
}

[Serializable]
public class WeaponInfo
{
    private AntiCheatInt _secureInt = new AntiCheatInt();
    private AntiCheatFloat _secureFloat = new AntiCheatFloat();

    public string Name { get; private set; }
    public WeaponType Type { get; private set; }
    public Color32 MainColor { get; private set; }
    public Sprite Icon { get; private set; }
    public Sprite SpecialSkillIcon { get; private set; }
    public Sprite ExSkillIcon { get; private set; }

    public float AttackDistance
    {
        get
        {
            if (!_secureFloat.Exist("attackDistance"))
            {
                Debug.LogError("Does not exist secureFloat: attackDistance");
                return -1;
            }

            return _secureFloat["attackDistance"];
        }
        set
        {
            if (!_secureFloat.Exist("attackDistance"))
            {
                Debug.LogError("Does not exist secureFloat: attackDistance");
                return;
            }

            _secureFloat["attackDistance"] = value;
        }
    }

    public float AttackDamage
    {
        get
        {
            if (!_secureFloat.Exist("attackDamage"))
            {
                Debug.LogError("Does not exist secureFloat: attackDamage");
                return -1;
            }

            return _secureFloat["attackDamage"];
        }
        set
        {
            if (!_secureFloat.Exist("attackDamage"))
            {
                Debug.LogError("Does not exist secureFloat: attackDamage");
                return;
            }

            _secureFloat["attackDamage"] = value;
        }
    }

    public int CurAmmo
    {
        get
        {
            if (!_secureInt.Exist("curAmmo"))
            {
                Debug.LogError("Does not exist secureInt: curAmmo");
                return -1;
            }

            return _secureInt["curAmmo"];
        }
        set
        {
            if (!_secureInt.Exist("curAmmo"))
            {
                Debug.LogError("Does not exist secureInt: curAmmo");
                return;
            }

            _secureInt["curAmmo"] = value;
        }
    }

    public int MaxAmmo
    {
        get
        {
            if (!_secureInt.Exist("maxAmmo"))
            {
                Debug.LogError("Does not exist secureInt: maxAmmo");
                return -1;
            }

            return _secureInt["maxAmmo"];
        }
        set
        {
            if (!_secureInt.Exist("maxAmmo"))
            {
                Debug.LogError("Does not exist secureInt: maxAmmo");
                return;
            }

            _secureInt["maxAmmo"] = value;
        }
    }

    public float CostResilience
    {
        get
        {
            if (!_secureFloat.Exist("costResilience"))
            {
                Debug.LogError("Does not exist secureFloat: costResilience");
                return -1;
            }

            return _secureFloat["costResilience"];
        }
        set
        {
            if (!_secureFloat.Exist("costResilience"))
            {
                Debug.LogError("Does not exist secureFloat: costResilience");
                return;
            }

            _secureFloat["costResilience"] = value;
        }
    }

    public float ReloadTime
    {
        get
        {
            if (!_secureFloat.Exist("reloadTime"))
            {
                Debug.LogError("Does not exist secureFloat: reloadTime");
                return float.MaxValue;
            }

            return _secureFloat["reloadTime"];
        }
        set
        {
            if (!_secureFloat.Exist("reloadTime"))
            {
                Debug.LogError("Does not exist secureFloat: reloadTime");
                return;
            }

            _secureFloat["reloadTime"] = value;
        }
    }

    public WeaponInfo()
    {
    }

    public WeaponInfo(WeaponInfoData data)
    {
        _secureFloat.Add("attackDistance");
        _secureFloat.Add("attackDamage");
        _secureInt.Add("curAmmo");
        _secureInt.Add("maxAmmo");
        _secureFloat.Add("costResilience");
        _secureFloat.Add("reloadTime");

        Name = data.Name;
        Type = data.Type;
        MainColor = data.MainColor;
        Icon = data.Icon;
        SpecialSkillIcon = data.SpecialSkillIcon;
        ExSkillIcon = data.ExSkillIcon;

        AttackDistance = data.BaseAttackDistance;
        AttackDamage = data.BaseAttackDamage;
        MaxAmmo = data.BaseMaxAmmo;
        CurAmmo = MaxAmmo;
        CostResilience = data.BaseCostResilience;
        ReloadTime = data.BaseReloadTime;
    }
}
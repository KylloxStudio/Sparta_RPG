using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    private AntiCheatInt _secureInt = new AntiCheatInt();
    private AntiCheatFloat _secureFloat = new AntiCheatFloat();

    public int Health
    {
        get
        {
            if (!_secureInt.Exist("hp"))
            {
                Debug.LogError("Does not exist secureInt: hp");
                return -1;
            }

            return _secureInt["hp"];
        }
        set
        {
            if (!_secureInt.Exist("hp"))
            {
                Debug.LogError("Does not exist secureInt: hp");
                return;
            }

            if (value >= MaxHealth)
            {
                _secureInt["hp"] = MaxHealth;
            }
            else if (value <= 0)
            {
                _secureInt["hp"] = 0;
            }
            else
            {
                _secureInt["hp"] = value;
            }
        }
    }

    public int MaxHealth
    {
        get
        {
            if (!_secureInt.Exist("maxHp"))
            {
                Debug.LogError("Does not exist secureInt: maxHp");
                return -1;
            }

            return _secureInt["maxHp"];
        }
        set
        {
            if (!_secureInt.Exist("maxHp"))
            {
                Debug.LogError("Does not exist secureInt: maxHp");
                return;
            }

            _secureInt["maxHp"] = value;
        }
    }

    public float MoveSpeed
    {
        get
        {
            if (!_secureFloat.Exist("moveSpeed"))
            {
                Debug.LogError("Does not exist secureFloat: moveSpeed");
                return -1f;
            }

            return _secureFloat["moveSpeed"];
        }
        set
        {
            if (!_secureFloat.Exist("moveSpeed"))
            {
                Debug.LogError("Does not exist secureFloat: moveSpeed");
                return;
            }

            _secureFloat["moveSpeed"] = value;
        }
    }

    public float Cost
    {
        get
        {
            if (!_secureFloat.Exist("cost"))
            {
                Debug.LogError("Does not exist secureFloat: cost");
                return -1f;
            }

            return _secureFloat["cost"];
        }
        private set
        {
            if (!_secureFloat.Exist("cost"))
            {
                Debug.LogError("Does not exist secureFloat: cost");
                return;
            }

            _secureFloat["cost"] = value;
        }
    }

    public float ExsCost
    {
        get
        {
            if (!_secureFloat.Exist("exsCost"))
            {
                Debug.LogError("Does not exist secureFloat: exsCost");
                return -1f;
            }

            return _secureFloat["exsCost"];
        }
        private set
        {
            if (!_secureFloat.Exist("exsCost"))
            {
                Debug.LogError("Does not exist secureFloat: exsCost");
                return;
            }

            _secureFloat["exsCost"] = value;
        }
    }

    public PlayerStats()
    {
    }

    public PlayerStats(PlayerStatsData data)
    {
        _secureInt.Add("hp");
        _secureInt.Add("maxHp");
        _secureFloat.Add("cost");
        _secureFloat.Add("exsCost");
        _secureFloat.Add("moveSpeed");

        MaxHealth = data.BaseMaxHealth;
        Health = MaxHealth;
        MoveSpeed = data.BaseMoveSpeed;
        Cost = 0f;
        ExsCost = data.ExsCost;
    }
}
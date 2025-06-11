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

    public float DefensePower
    {
        get
        {
            if (!_secureFloat.Exist("defense"))
            {
                Debug.LogError("Does not exist secureFloat: defense");
                return -1f;
            }

            return _secureFloat["defense"];
        }
        set
        {
            if (!_secureFloat.Exist("defense"))
            {
                Debug.LogError("Does not exist secureFloat: defense");
                return;
            }

            _secureFloat["defense"] = value;
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
        set
        {
            if (!_secureFloat.Exist("cost"))
            {
                Debug.LogError("Does not exist secureFloat: cost");
                return;
            }

            if (value >= ExSkillCost)
            {
                _secureFloat["cost"] = ExSkillCost;
            }
            else if (value <= 0f)
            {
                _secureFloat["cost"] = 0f;
            }
            else
            {
                _secureFloat["cost"] = value;
            }
        }
    }

    public float SpecialSkillCooltime
    {
        get
        {
            if (!_secureFloat.Exist("specialCool"))
            {
                Debug.LogError("Does not exist secureFloat: specialCool");
                return float.MaxValue;
            }

            return _secureFloat["specialCool"];
        }
        private set
        {
            if (!_secureFloat.Exist("specialCool"))
            {
                Debug.LogError("Does not exist secureFloat: specialCool");
                return;
            }

            _secureFloat["specialCool"] = value;
        }
    }

    public float ExSkillCost
    {
        get
        {
            if (!_secureFloat.Exist("exsCost"))
            {
                Debug.LogError("Does not exist secureFloat: exsCost");
                return float.MaxValue;
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
        _secureFloat.Add("defense");
        _secureFloat.Add("cost");
        _secureFloat.Add("specialCool");
        _secureFloat.Add("exsCost");
        _secureFloat.Add("moveSpeed");

        MaxHealth = data.BaseMaxHealth;
        Health = MaxHealth;
        DefensePower = data.BaseDefensePower;
        MoveSpeed = data.BaseMoveSpeed;
        Cost = 0f;
        SpecialSkillCooltime = data.SpecialSkillCooltime;
        ExSkillCost = data.ExSkillCost;
    }
}
using System;
using UnityEngine;

[Serializable]
public class EnemyStats
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

    public EnemyStats()
    {
    }

    public EnemyStats(EnemyStatsData data)
    {
        _secureInt.Add("hp");
        _secureInt.Add("maxHp");
        _secureFloat.Add("defense");
        _secureFloat.Add("attackDistance");
        _secureFloat.Add("attackDamage");
        _secureFloat.Add("moveSpeed");

        MaxHealth = data.BaseMaxHealth;
        Health = MaxHealth;
        DefensePower = data.BaseDefensePower;
        AttackDistance = data.BaseAttackDistance;
        AttackDamage = data.BaseAttackDamage;
        MoveSpeed = data.BaseMoveSpeed;
    }
}
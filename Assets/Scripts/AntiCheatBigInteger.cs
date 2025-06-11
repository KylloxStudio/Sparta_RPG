using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class AntiCheatBigInteger
{
    private Dictionary<string, AntiCheatValue> _values = new Dictionary<string, AntiCheatValue>();

    private class AntiCheatValue
    {
        private int _key = Random.Range(1, 1000000);
        private BigInteger _lockedValue;

        public BigInteger Value
        {
            get
            {
                return _lockedValue ^ _key;
            }
            set
            {
                _key = Random.Range(1, 1000000);
                _lockedValue = value ^ _key;
            }
        }

        public AntiCheatValue(BigInteger value = default)
        {
            Value = value;
        }
    }

    public bool Add(string name, BigInteger value = default)
    {
        if (_values.ContainsKey(name))
        {
            return false;
        }
        else
        {
            _values.Add(name, new AntiCheatValue(value));
            return true;
        }
    }

    public bool Remove(string name)
    {
        if (_values.ContainsKey(name))
        {
            _values.Remove(name);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Exist(string name)
    {
        return _values.ContainsKey(name);
    }

    public BigInteger this[string name]
    {
        get => _values[name].Value;
        set => _values[name].Value = value;
    }
}
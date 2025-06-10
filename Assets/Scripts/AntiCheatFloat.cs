using System;
using System.Collections.Generic;

public class AntiCheatFloat
{
    private Dictionary<string, AntiCheatValue> _values = new Dictionary<string, AntiCheatValue>();

    private class AntiCheatValue
    {
        private int _key = UnityEngine.Random.Range(1, 1000000);
        private float _lockedValue;

        public float Value
        {
            get
            {
                int intValue = FloatToInt(_lockedValue);
                return IntToFloat(intValue ^ _key);
            }
            set
            {
                _key = UnityEngine.Random.Range(1, 1000000);
                int intValue = FloatToInt(value);
                _lockedValue = IntToFloat(intValue ^ _key);
            }
        }

        private int FloatToInt(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        private float IntToFloat(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        public AntiCheatValue(float value = default)
        {
            Value = value;
        }
    }

    public bool Add(string name, float value = default)
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

    public float this[string name]
    {
        get => _values[name].Value;
        set => _values[name].Value = value;
    }
}
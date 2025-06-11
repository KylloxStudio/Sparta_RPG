using System;
using System.Numerics;
using UnityEngine;

public class BalanceEvent : UnityEngine.Events.UnityEvent<BigInteger, BigInteger, bool> { };

public class PlayerBalance : MonoBehaviour
{
    private AntiCheatBigInteger _secureBigInteger = new AntiCheatBigInteger();

    public BalanceEvent OnBalanceUpdated = new BalanceEvent();

    private bool _isFormatted = true;

    public BigInteger Pyroxenes
    {
        get
        {
            if (!_secureBigInteger.Exist("pyroxenes"))
            {
                Debug.LogError("Does not exist secureInt: pyroxenes");
                return -1;
            }

            return _secureBigInteger["pyroxenes"];
        }
        private set
        {
            if (!_secureBigInteger.Exist("pyroxenes"))
            {
                Debug.LogError("Does not exist secureInt: pyroxenes");
                return;
            }

            _secureBigInteger["pyroxenes"] = value;
        }
    }

    private void Awake()
    {
        _secureBigInteger.Add("pyroxenes");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _isFormatted = !_isFormatted;
            OnBalanceUpdated?.Invoke(Pyroxenes, Pyroxenes, _isFormatted);
        }
    }

    public void AddPyroxenes(BigInteger value)
    {
        BigInteger prev = Pyroxenes;
        Pyroxenes += value;
        OnBalanceUpdated?.Invoke(prev, Pyroxenes, _isFormatted);
    }

    public void SetPyroxenes(BigInteger value)
    {
        BigInteger prev = Pyroxenes;
        Pyroxenes = value;
        OnBalanceUpdated?.Invoke(prev, Pyroxenes, _isFormatted);
    }
}
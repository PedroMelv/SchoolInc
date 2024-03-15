using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameCurrency : Singleton<GameCurrency>
{
    public ulong debug_StartMoney;


    private BigInteger currency;
    public BigInteger Currency
    {
        get => currency;
        set
        {
            currency = value;
            OnCurrencyChanged?.Invoke(currency);
            OnCurrencyChanged_String?.Invoke(CurrencyString);
        }
    }

    public Action<BigInteger> OnCurrencyChanged;
    public Action<string> OnCurrencyChanged_String;

    
    public string CurrencyString
    {
        get => MoneyUtils.MoneyString(currency);
    }

    private void Start()
    {
        AddCurrency(debug_StartMoney);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) AddCurrency(10);
    }

    public void AddCurrency(BigInteger value)
    {
        Currency += value;
    }

    public void RemoveCurrency(BigInteger value)
    {
        RemoveCurrency(value,null,null);
    }
    public void RemoveCurrency(BigInteger value, Action onFailBuy, Action onSuccessBuy)
    {
        if (currency - value < 0)
        {
            onFailBuy?.Invoke();
            return;
        }
        Currency -= value;
        onSuccessBuy?.Invoke();
    }

    
}

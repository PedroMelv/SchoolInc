using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameCurrency : Singleton<GameCurrency>, IBind<GameCurrency.CurrencyData>
{
    public string debug_StartMoney;

    private BigInteger coinCurrency;
    public BigInteger CoinCurrency
    {
        get => coinCurrency;
        set
        {
            coinCurrency = value;
            OnCurrencyChanged?.Invoke(coinCurrency, coinCurrency);
            OnCurrencyChanged_String?.Invoke(CurrencyString, CoinCurrencyString);
        }
    }
    public string CoinCurrencyString
    {
        get => MoneyUtils.MoneyString(coinCurrency);
    }
    
    private BigInteger currency;
    public BigInteger Currency
    {
        get => currency;
        set
        {
            currency = value;
            OnCurrencyChanged?.Invoke(coinCurrency, coinCurrency);
            OnCurrencyChanged_String?.Invoke(CurrencyString, CoinCurrencyString);
        }
    }

    public Action<BigInteger, BigInteger> OnCurrencyChanged;
    public Action<string, string> OnCurrencyChanged_String;

    public string CurrencyString
    {
        get => MoneyUtils.MoneyString(currency, "$");
    }
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [SerializeField] private CurrencyData data;

    private void Start()
    {
        OnCurrencyChanged?.Invoke(currency, coinCurrency);
        OnCurrencyChanged_String?.Invoke(CurrencyString, CoinCurrencyString);
    }

    private void Update()
    {
        data.currency = currency.ToString();
        data.coinCurrency = coinCurrency.ToString();
        if (Input.GetKey(KeyCode.Alpha1)) AddCurrency(10d);
    }

    public void AddCurrency(BigInteger value)
    {
        Currency += value;
    }
    public void AddCurrency(double value)
    {
        AddCurrency(new BigInteger(value));
    }
    
    public void RemoveCurrency(BigInteger value)
    {
        RemoveCurrency(value,null,null);
    }
    public void RemoveCurrency(double value)
    {
        RemoveCurrency(new BigInteger(value), null, null);
    }
    public void RemoveCurrency(double value, Action onFailBuy, Action onSuccessBuy)
    {
        RemoveCurrency(new BigInteger(value), onFailBuy, onSuccessBuy);
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

    public void Bind(CurrencyData data)
    {
        this.data = data;
        this.data.Id = Id;
        if (BigInteger.TryParse(data.currency, out BigInteger currency))
            Currency = currency;
        else 
            Currency = 0;

        if (BigInteger.TryParse(data.coinCurrency, out currency))
            CoinCurrency = currency;
        else 
            CoinCurrency = 0;
    }

    [Serializable]
    public class CurrencyData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public string currency;
        public string coinCurrency;

        public void Reset()
        {
            currency = "reseted";
            coinCurrency = "reseted";
        }
    }
}

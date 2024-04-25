using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameCurrency : Singleton<GameCurrency>, IBind<GameCurrency.CurrencyData>
{
    public string debug_StartMoney;
    public string currencyToOneAscended_string;
    public double currencyToOneAscendedMultiplier;

    private BigInteger coinCurrency;
    public BigInteger CoinCurrency
    {
        get => coinCurrency;
        set
        {
            data.coinCurrency = value.ToString();
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

    private BigInteger totalCurrency;
    private BigInteger currencyToOneAscended;



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
        data.currencyToOneAscended = currencyToOneAscended.ToString();
        data.totalCurrency = totalCurrency.ToString();

        while(currencyToOneAscended < totalCurrency)
        {
            Debug.Log("Adding ascended currency");
            AddCurrencyAscended(new BigInteger(1));
            currencyToOneAscended += currencyToOneAscended;
            currencyToOneAscended = new BigInteger((double)currencyToOneAscended * currencyToOneAscendedMultiplier);
        }

        if (Input.GetKey(KeyCode.Alpha1)) AddCurrency(10d);
        if (Input.GetKey(KeyCode.Alpha2)) coinCurrency += 10;
    }

    public void AddCurrency(BigInteger value)
    {
        Debug.Log("Adding currency");
        Currency += value;
        totalCurrency += value;
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
    

    public void AddCurrencyAscended(BigInteger value)
    {
        CoinCurrency += value;
    }
    public void AddCurrencyAscended(double value)
    {
        AddCurrencyAscended(new BigInteger(value));
    }
    public void RemoveCurrencyAscended(BigInteger value)
    {
        RemoveCurrencyAscended(value, null, null);
    }
    public void RemoveCurrencyAscended(double value)
    {
        RemoveCurrencyAscended(new BigInteger(value), null, null);
    }
    public void RemoveCurrencyAscended(double value, Action onFailBuy, Action onSuccessBuy)
    {
        RemoveCurrencyAscended(new BigInteger(value), onFailBuy, onSuccessBuy);
    }
    public void RemoveCurrencyAscended(BigInteger value, Action onFailBuy, Action onSuccessBuy)
    {
        if (coinCurrency - value < 0)
        {
            onFailBuy?.Invoke();
            return;
        }
        CoinCurrency -= value;
        onSuccessBuy?.Invoke();
    }



    public void Bind(CurrencyData data)
    {
        this.data = data;
        this.data.Id = Id;

        if (BigInteger.TryParse(data.totalCurrency, out BigInteger currency))
            totalCurrency = currency;
        else
            totalCurrency = 0;

        if (BigInteger.TryParse(data.currency, out currency))
            Currency = currency;
        else 
            Currency = 0;

        if (BigInteger.TryParse(data.coinCurrency, out currency))
            CoinCurrency = currency;
        else 
            CoinCurrency = 0;

        if (BigInteger.TryParse(data.currencyToOneAscended, out currency))
            currencyToOneAscended = currency;
        else
            if (BigInteger.TryParse(currencyToOneAscended_string, out currency))
                currencyToOneAscended = currency;
            
    }

    public void ResetData()
    {
        data.Reset();
    }

    [Serializable]
    public class CurrencyData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public string currency;
        public string coinCurrency;
        public string currencyToOneAscended;
        public string totalCurrency;
        public void Reset()
        {
            currency = "reseted";
            coinCurrency = "reseted";
            currencyToOneAscended = "reseted";
            totalCurrency = "reseted";
        }

        public void Reset_Ascended()
        {
            currency = "reseted";
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameCurrency : Singleton<GameCurrency>
{
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

    private int suffixCounter;
    public string CurrencyString
    {
        get 
        {
            int max = 4;
            if(max > currency.ToString().Length) max = currency.ToString().Length;

            string number = currency.ToString("N0");
            string[] splitted = number.Split('.');

            if (splitted == null || splitted.Length == 0) return "$" + number + GetSuffix();

            return "$" + splitted[0] + GetSuffix();
        }
    }

    private Dictionary<BigInteger, string> suffix = new Dictionary<BigInteger, string>
        {
            { 4 , "K" },
            { 7 , "M" },
            { 10, "B" },
            { 13, "T" },
            { 16, "Q" },
            { 19, "Qi" },
            { 21, "Se" },
            { 24, "Sp" },
            { 27, "Oc" },
            { 30, "No" },
            { 33, "Dc" }
        };

    private void Start()
    {
        AddCurrency(0);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) AddCurrency(10);
    }

    public void AddCurrency(BigInteger value)
    {
        Currency += value;
    }

    private string GetSuffix()
    {
        if(suffixCounter == currency.ToString().Length)
        {
            return suffix.GetValueOrDefault(suffixCounter, string.Empty);
        }

        string result = suffix.GetValueOrDefault(currency.ToString().Length, string.Empty);
        int extraTab = 4;

        if (!string.IsNullOrEmpty(result))
        {
            suffixCounter = currency.ToString().Length;
            return result;
        }

        while (extraTab > 0 && string.IsNullOrEmpty(result))
        {
            extraTab--;
            result = suffix.GetValueOrDefault(currency.ToString().Length - extraTab, string.Empty);
        }

        suffixCounter = currency.ToString().Length - extraTab;
        return result;
    }
}

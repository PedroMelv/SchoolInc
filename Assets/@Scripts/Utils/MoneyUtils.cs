using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MoneyUtils
{
    private static Dictionary<BigInteger, string> suffix = new Dictionary<BigInteger, string>
        {
            //{ 4 , "K" },
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

    private static string GetSuffix(BigInteger value)
    {
        string result = suffix.GetValueOrDefault(value.ToString().Length, string.Empty);
        int extraTab = 4;

        if (!string.IsNullOrEmpty(result))
        {
            return result;
        }

        while (extraTab > 0 && string.IsNullOrEmpty(result))
        {
            extraTab--;
            result = suffix.GetValueOrDefault(value.ToString().Length - extraTab, string.Empty);
        }

        return result;
    }

    public static string MoneyString(BigInteger value, string suffix)
    {
        string number = value.ToString("N0");
        string fullNumber = number;

        number = fullNumber.Substring(0, Mathf.Min(number.Length, 7));
        if (number[number.Length - 1] == '.') number = number.Remove(number.Length - 1);


        return suffix + number + GetSuffix(value);
    }

    public static string MoneyString(BigInteger value)
    {
        return MoneyString(value, "");
    }


}

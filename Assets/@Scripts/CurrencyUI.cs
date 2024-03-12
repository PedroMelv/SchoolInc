using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;

    private void Start()
    {
        GameCurrency.Instance.OnCurrencyChanged_String += UpdateCurrencyText;
    }

    private void UpdateCurrencyText(string text)
    {
        Debug.Log(text);
        currencyText.SetText(text);
    }
}

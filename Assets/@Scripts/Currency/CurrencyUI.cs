using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private GameCurrency gameCurrency;

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI coinsText;

    private void Start()
    {
        gameCurrency.OnCurrencyChanged_String += UpdateCurrencyText;
    }

    private void OnDisable()
    {
        gameCurrency.OnCurrencyChanged_String -= UpdateCurrencyText;
    }

    private void UpdateCurrencyText(string currency, string coins)
    {
        coinsText.SetText(coins);
        currencyText.SetText(currency);
    }
}

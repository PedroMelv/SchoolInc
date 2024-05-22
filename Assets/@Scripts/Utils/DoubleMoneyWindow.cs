using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoubleMoneyWindow : StaticInstance<DoubleMoneyWindow>
{
    [SerializeField] private GameCurrency gameCurrency;
    private double moneyAmount;
    public double MoneyAmount { 
        get => moneyAmount;//
        set
        {
            moneyAmount = value;
            moneyText.text = "Você ganhou " + MoneyUtils.MoneyString(moneyAmount, "+R$") + " deseja dobrar?";
        }
    }

    [SerializeField] private GameObject doubleMoneyWindow;
    [SerializeField] private TextMeshProUGUI moneyText;

    public void OfferDoubleMoney(double moneyAmount)
    {
        doubleMoneyWindow.SetActive(true);
        MoneyAmount = moneyAmount;
    }

    public void DoubleMoneyButton()
    {
        AdsSystem.PlayRewarded(() =>
        {
            MoneyAmount *= 2f;
            CloseWindow();
        });
    }

    public void CloseWindow()
    {
        gameCurrency.AddCurrency(MoneyAmount);
        doubleMoneyWindow.SetActive(false);
    }
}

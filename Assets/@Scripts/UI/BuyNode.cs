using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyNode : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI quantityText;

    private Func<(string, string)> OnBuyUpdate;

    public void Initialize(string upgradeName, string quantity, string price, Action OnBuyAction, Func<(string,string)> OnBuyUpdate)
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuyAction?.Invoke());
        buyButton.onClick.AddListener(() => OnBuy());

        nameText.SetText(upgradeName);
        priceText.SetText(price);
        quantityText.SetText(quantity);

        this.OnBuyUpdate = OnBuyUpdate;
    }

    private void OnBuy()
    {
        if (OnBuyUpdate == null) return;

        (string price, string quantity) = OnBuyUpdate.Invoke();

        priceText.SetText(price);
        quantityText.SetText(quantity);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyNode : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI quantityText;

    private UpgradeDatabase.Upgrade upgrade;

    public Action onButtonClickCallback;

    public void Initialize(UpgradeDatabase.Upgrade upgrade)
    {
        string quantity = upgrade.currentQuantity + "x";
        string price = MoneyUtils.MoneyString(upgrade.Price);
        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        nameText.SetText(upgrade.name);

        buyButton.interactable = !isMaxed;

        if (isMaxed)
        {
            quantityText.SetText("Maxed");
            priceText.SetText("Maxed");
            return;
        }

        this.upgrade = upgrade;

        priceText.SetText(price);
        quantityText.SetText(quantity);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => GameCurrency.Instance.RemoveCurrency(upgrade.Price, null, OnBuy));
        buyButton.onClick.AddListener(() => onButtonClickCallback?.Invoke());
    }

    private void OnBuy()
    {
        upgrade.currentQuantity++;

        if (upgrade.triggerAutomatic) 
            SchoolsManager.Instance.SchoolSelected.Tappable.Automatic.SetInfinity(true);
    }
}

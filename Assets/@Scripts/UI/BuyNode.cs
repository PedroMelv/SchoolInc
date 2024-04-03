using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BuyNode : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private SliderUI quantitySlider;

    private float scaling;
    public bool interactable 
    { 
        get
        {
            return buyButton.interactable;
        }
        
        set
        {
            buyButton.interactable = value;
        }
    }

    private UpgradeDatabase.Upgrade upgrade;
    public UpgradeDatabase.Upgrade Upgrade
    {
        get => upgrade;
    }

    public Action onButtonClickCallback;

    public void Initialize(UpgradeDatabase.Upgrade upgrade, float scaling = 1f)
    {
        this.scaling = scaling;
        this.upgrade = upgrade;

        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        UpdateNodeUI();

        if (isMaxed) return;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => GameCurrency.Instance.RemoveCurrency(new BigInteger((double)upgrade.Price * scaling), null, OnBuy));
        buyButton.onClick.AddListener(() => onButtonClickCallback?.Invoke());
        //buyButton.onClick.AddListener(UpdateNodeUI);
    }

    private void UpdateNodeUI()
    {
        string quantity = upgrade.currentQuantity + "x";
        string price = MoneyUtils.MoneyString(new BigInteger((double)upgrade.Price * scaling), "$");
        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        nameText.SetText(upgrade.name);

        buyButton.interactable = !isMaxed;

        if (isMaxed)
        {
            quantitySlider.SetFill(1f);
            priceText.SetText("Maxed");
            return;
        }

        priceText.SetText(price);
        quantitySlider.SetFill(upgrade.currentQuantity, upgrade.maxQuantity);
    }

    private void OnBuy()
    {
        SchoolData data = SchoolsManager.Instance.SchoolSelected;

        data.OnBuy(upgrade);

    }

}

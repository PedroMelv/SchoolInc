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
    [SerializeField] protected Button buyButton;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI priceText;
    [SerializeField] protected SliderUI quantitySlider;
    public string suffix = "$";

    protected float scaling;
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

    protected UpgradeDatabase.Upgrade upgrade;
    public UpgradeDatabase.Upgrade Upgrade
    {
        get => upgrade;
    }

    public Action onButtonClickCallback;

    public virtual void Initialize(UpgradeDatabase.Upgrade upgrade, float scaling = 1f)
    {
        this.scaling = scaling;
        this.upgrade = upgrade;

        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        UpdateNodeUI();

        if (isMaxed) return;

        InitializeOnBuyEvent();
    }

    protected virtual void InitializeOnBuyEvent()
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => GameCurrency.Instance.RemoveCurrency(new BigInteger((double)upgrade.Price * scaling), null, OnBuy));
        buyButton.onClick.AddListener(() => onButtonClickCallback?.Invoke());
    }

    protected virtual void UpdateNodeUI()
    {
        string quantity = upgrade.currentQuantity + "x";
        string price = MoneyUtils.MoneyString(new BigInteger((double)upgrade.Price * scaling), suffix);
        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        nameText.SetText(upgrade.name);

        buyButton.interactable = !isMaxed;

        if (isMaxed)
        {
            quantitySlider.SetFill(1f);
            priceText.SetText("Máximo");
            return;
        }

        priceText.SetText(price);
        quantitySlider.SetFill(upgrade.currentQuantity, upgrade.maxQuantity);
    }

    protected virtual void OnBuy()
    {
        
    }

}

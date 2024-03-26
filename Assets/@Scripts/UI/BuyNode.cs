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
    [SerializeField] private TextMeshProUGUI quantityText;

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

    public void Initialize(UpgradeDatabase.Upgrade upgrade)
    {
        this.upgrade = upgrade;

        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        UpdateNodeUI();

        if (isMaxed) return;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => GameCurrency.Instance.RemoveCurrency(upgrade.Price, null, OnBuy));
        buyButton.onClick.AddListener(() => onButtonClickCallback?.Invoke());
        //buyButton.onClick.AddListener(UpdateNodeUI);
    }

    private void UpdateNodeUI()
    {
        string quantity = upgrade.currentQuantity + "x";
        string price = MoneyUtils.MoneyString(upgrade.Price, "$");
        bool isMaxed = upgrade.currentQuantity >= upgrade.maxQuantity;

        nameText.SetText(upgrade.name);

        buyButton.interactable = !isMaxed;

        if (isMaxed)
        {
            quantityText.SetText("Maxed");
            priceText.SetText("Maxed");
            return;
        }

        priceText.SetText(price);
        quantityText.SetText(quantity);
    }

    private void OnBuy()
    {
        SchoolData data = SchoolsManager.Instance.SchoolSelected;
        upgrade.currentQuantity++;

        if (upgrade.triggerAutomatic)
            data.Tappable.Automatic.SetInfinity(true);


        switch (upgrade.upgradeType)
        {
            case UpgradeType.REVENUE:
                data.initialRevenue = HandleIncrease(data.initialRevenue);
                break;
            case UpgradeType.INCOME:
                data.incomeMultiplier = HandleIncrease(data.incomeMultiplier);
                break;
            case UpgradeType.STUDENT:
                data.studentsCount = HandleIncrease(data.studentsCount);
                break;
            case UpgradeType.TEACHER:
                data.fillTimeSpeed = HandleIncrease(data.fillTimeSpeed);
                break;
            case UpgradeType.SLIDER_SPEED:
                data.fillTimeSpeed = HandleIncrease(data.fillTimeSpeed);
                break;
            case UpgradeType.HOLDING_INCREASE:
                data.maxMoneyHold = HandleIncrease(data.maxMoneyHold);
                break;
            case UpgradeType.TAPBOOST_AMOUNT:
                data.tapBoostMax = HandleIncrease(data.tapBoostMax);
                break;
            case UpgradeType.TAPBOOST_STRENGTH:
                data.tapBoostStrength = HandleIncrease(data.tapBoostStrength);
                break;
            case UpgradeType.TAPBOOST_SPEED:
                data.tapBoostFillSpeed = HandleIncrease(data.tapBoostFillSpeed);
                break;
            default:
                break;
        }
    }

    private BigInteger HandleIncrease(BigInteger value)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return value + (BigInteger)upgrade.increaseValue;
            case IncreaseType.PERCENTAGE:
                return new BigInteger((double)value * (1 + (upgrade.increaseValue / 100f)));
            case IncreaseType.MULTIPLY:
                return new BigInteger((double)value * upgrade.increaseValue);
        }
        return value;
    }

    private int HandleIncrease(int value)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return Mathf.RoundToInt(value + upgrade.increaseValue);
            case IncreaseType.PERCENTAGE:
                return Mathf.RoundToInt(value * (1 + upgrade.increaseValue / 100));
            case IncreaseType.MULTIPLY:
                return Mathf.RoundToInt(value * upgrade.increaseValue);
        }
        return value;
    }

    private float HandleIncrease(float value)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return value + upgrade.increaseValue;
            case IncreaseType.PERCENTAGE:
                return value * (1 + upgrade.increaseValue / 100);
            case IncreaseType.MULTIPLY:
                return value * upgrade.increaseValue;
        }
        return value;
    }
}

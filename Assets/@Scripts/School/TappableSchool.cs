using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TappableSchool : TappableObject
{
    private float fillCurrent;
    private int tapCount;
    public int TapCount { get => tapCount; set => tapCount = value; }

    private float tapTimer = 1f;
    public float TapTimer { get => tapTimer; set => tapTimer = value; }

    private float tapRecoverTimer = 1;
    private bool tapCooldown = false;

    private SchoolData data;
    private SchoolVisual visual;

    public Action<int, int> OnTapCountChanged;

    protected override void Awake()
    {
        base.Awake();
        data = GetComponent<SchoolData>();
        visual = GetComponent<SchoolVisual>();
    }
    protected override void Start()
    {
        base.Start();
        onComplete += data.EarnMoney;
        onComplete += () => visual.SetCollectButtonActive(true, MoneyUtils.MoneyString(data.holdingMoney, "$"));
        visual.AddCollectButtonEvent(CollectMoney);
    }

    private void Update()
    {
        RegenerateTaps();
    }

    private void RegenerateTaps()
    {
        if (!data.isUnlocked) return;

        if (tapCount <= 0)
        {
            tapCooldown = false;
            tapRecoverTimer = 1;
            return;
        }

        if (tapCooldown)
        {
            tapTimer -= Time.deltaTime * data.tapBoostFillSpeed;
            if (tapTimer < 0f)
            {
                tapCount -= 1;
                OnTapCountChanged?.Invoke(tapCount, data.tapBoostMax);
                tapTimer = 1f;
            }
            return;
        }


        tapRecoverTimer -= Time.deltaTime;
        if (tapRecoverTimer <= 0f)
        {
            tapTimer -= Time.deltaTime * data.tapBoostFillSpeed;
            if (tapTimer < 0f)
            {
                tapCount -= 1;
                OnTapCountChanged?.Invoke(tapCount, data.tapBoostMax);
                tapTimer = 1f;
            }
        }
        
    }

    public void UpdateTapCount()
    {
        OnTapCountChanged?.Invoke(tapCount, data.tapBoostMax);
    }

    private void BuySchool()
    {
        GameCurrency.Instance.RemoveCurrency(data.initialCost, OnFailBuy, OnSucceedBuy);
    }

    private void OnFailBuy()
    {
        //TODO: Criar o método caso a compra venha a falhar
        Debug.LogError("Not enought cash. Cost: " + data.initialCost);
    }
    private void OnSucceedBuy()
    {
        //TODO: SFX e Efeitos visuais de compra
        visual.SetCostText(false);
        visual.SetProgressionSlider(true);
        visual.SetIsUnlocked(true);
        data.isUnlocked = true;

        SchoolsManager.Instance.SchoolSelected = data;
        SchoolsManager.Instance.boughtSchools.Add(data);
    }


    public void CalculateTime(double seconds)
    {
        float timeToFill = 1f / data.tapBoostFillSpeed;

        double timesFilled = seconds / timeToFill;

        int filledAmount = Mathf.FloorToInt((float)timesFilled);
        float extraFilled = (float)timesFilled - filledAmount;

        tapCount -= filledAmount;
        if(tapCount < 0) tapCount = 0;
        
    }

    #region Income_Generators
    public override void Tap(bool useShrink = true)
    {
        if (data.isUnlocked == false)
        {
            BuySchool();
            return;
        }
        SchoolsManager.Instance.SchoolSelected = data;

        bool tapWillWork = true;
        if (canTap != null) { tapWillWork = canTap.Invoke(); }

        if (tapCount >= data.tapBoostMax || !tapWillWork || tapCooldown) return;

        tapRecoverTimer = 1;


        base.Tap(useShrink);

        fillCurrent += 1 + data.TimeToFill * data.tapBoostStrength;
        tapCount++;
        OnTapCountChanged?.Invoke(tapCount, data.tapBoostMax);
        UpdateSlider();

        if(tapCount == data.tapBoostMax) tapCooldown = true;
    }

    public override void TapWithTime()
    {
        if (data.isUnlocked == false) return;
        base.TapWithTime();

        fillCurrent += Time.deltaTime * data.fillTimeSpeed;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        float fillPercentage = fillCurrent / data.TimeToFill;

        if (fillPercentage >= 1f)
        {
            fillCurrent = 0;
            fillPercentage = 0;

            visual.SetProgressionSliderForced(0f);

            onComplete?.Invoke();
        }

        visual.SetProgressionSlider(fillPercentage);
    }

    

    public void CollectMoney()
    {
        if (UnityEngine.Random.value < .05f)
        {
            DoubleMoneyWindow.Instance.OfferDoubleMoney((double)data.holdingMoney);
            data.holdingMoney = 0;
            visual.SetCollectButtonActive(false);
            return;
        }
        Debug.Log("Collecting Money: " + data.holdingMoney);
        GameCurrency.Instance.AddCurrency(data.holdingMoney);
        data.holdingMoney = 0;
        visual.SetCollectButtonActive(false);
    }

    #endregion
}

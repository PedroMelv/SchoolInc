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
    private int tapMax = 10;

    private float tapTimer = 1f;

    private float tapRecoverTimer = 1;
    private bool tapCooldown = false;

    private SchoolData data;
    private SchoolVisual visual;

    public Action<int, int> OnTapCountChanged;

    private void Awake()
    {
        data = GetComponent<SchoolData>();
        visual = GetComponent<SchoolVisual>();
    }
    protected override void Start()
    {
        base.Start();
        onComplete += CalculateMoney;
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
            tapTimer -= Time.deltaTime;
            if (tapTimer < 0f)
            {
                tapCount -= 1;
                OnTapCountChanged?.Invoke(tapCount, tapMax);
                tapTimer = 1f;
            }
            return;
        }


        tapRecoverTimer -= Time.deltaTime;
        if (tapRecoverTimer <= 0f)
        {
            tapTimer -= Time.deltaTime;
            if (tapTimer < 0f)
            {
                tapCount -= 1;
                OnTapCountChanged?.Invoke(tapCount, tapMax);
                tapTimer = 1f;
            }
        }
        
    }

    public void UpdateTapCount()
    {
        OnTapCountChanged?.Invoke(tapCount, tapMax);
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
        data.studentCount = 1;
        data.isUnlocked = true;

        SchoolsManager.Instance.boughtSchools.Add(data);
    }

    #region Income_Generators
    public override void Tap(bool useShrink = true)
    {
        SchoolsManager.Instance.SchoolSelected = data;

        if (data.isUnlocked == false)
        {
            BuySchool();
            return;
        }

        bool tapWillWork = true;
        if (canTap != null) { tapWillWork = canTap.Invoke(); }

        if (tapCount >= tapMax || !tapWillWork || tapCooldown) return;

        tapRecoverTimer = 1;


        base.Tap(useShrink);

        fillCurrent += 1 + data.TimeToFill * .01f;
        tapCount++;
        OnTapCountChanged?.Invoke(tapCount, tapMax);
        UpdateSlider();

        if(tapCount == tapMax) tapCooldown = true;
    }

    public override void TapWithTime()
    {
        if (data.isUnlocked == false) return;
        base.TapWithTime();
        fillCurrent += Time.deltaTime * data.assistantMultiplier;
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

    private void CalculateMoney()
    {
        BigInteger moneyMade = (((BigInteger)data.initialRevenue * (BigInteger)data.studentCount) * (BigInteger)data.directorMultiplier);

        GameCurrency.Instance.AddCurrency(moneyMade);
    }
    #endregion
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TappableSchool : TappableObject
{
    [SerializeField] private GameObject schoolSliderPrefab;
    private Transform sliderSchool;

    private float fillCurrent;
    private int tapCount;
    private int tapMax = 10;

    private float timer = 1f;

    private SchoolData data;

    private void Awake()
    {
        data = GetComponent<SchoolData>();
    }
    protected override void Start()
    {
        GameObject schoolObj = Instantiate(schoolSliderPrefab, transform.position + Vector3.up * 13, Quaternion.identity);
        sliderSchool = schoolObj.transform.Find("Fill");
        sliderSchool.localScale = new Vector3(0f, 1f, 1f);

        onComplete += CalculateMoney;
    }

    private void Update()
    {
        if (tapCount > 0)
            if (timer <= 0f)
            {
                tapCount -= 1;
                timer = 1f;
            }
            else timer -= Time.deltaTime;
    }

    public override void Tap(bool useShrink = true)
    {
        bool tapWillWork = true;
        if (canTap != null) { tapWillWork = canTap.Invoke(); }

        if (!tapWillWork) return;

        if (tapCount >= tapMax)
        {
            return;
        }
        base.Tap(useShrink);

        fillCurrent += 1 + data.TimeToFill * .01f;
        tapCount++;
        UpdateSlider();
    }

    public override void TapWithTime()
    {
        base.TapWithTime();
        fillCurrent += Time.deltaTime;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        float fillPercentage = fillCurrent / data.TimeToFill;

        if (fillPercentage >= 1f)
        {
            fillCurrent = 0;
            fillPercentage = 0;

            sliderSchool.localScale = new Vector3(0f, 1f, 1f);

            onComplete?.Invoke();
        }

        sliderSchool.DOKill(true);
        sliderSchool.DOScaleX(fillPercentage, .1f);
    }

    private void CalculateMoney()
    {
        int moneyMade = data.studentCount * 100;

        GameCurrency.Instance.AddCurrency(moneyMade);
    }
}

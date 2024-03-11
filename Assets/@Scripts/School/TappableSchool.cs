using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TappableSchool : TappableObject
{
    [SerializeField] private GameObject schoolSliderPrefab;
    private Transform sliderSchool;

    private int tapCounter;

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
    }

    public override void Tap(bool useShrink = true)
    {
        if (data.TapMax == 0) return;

        base.Tap(useShrink);
        tapCounter++;

        float tapPercentage = (float)tapCounter / data.TapMax;

        if(tapPercentage >= 1f)
        {
            tapCounter = 0;
            tapPercentage = 0;

            sliderSchool.localScale = new Vector3(0f, 1f, 1f);

        }

        sliderSchool.DOKill(true);
        sliderSchool.DOScaleX(tapPercentage, .1f);
    }
}

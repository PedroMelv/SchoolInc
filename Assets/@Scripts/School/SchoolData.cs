using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolData : MonoBehaviour
{
    public bool isUnlocked;
    [Space]

    public int initialCost = 1;
    public int initialRevenue = 1;
    public float incomeMultiplier = 1;
    public ulong maxMoneyHold = 500;

    [Space]
    [Range(0f, 1f)] public float tapBoostStrength = .01f;
    public int tapBoostMax = 10;
    public float tapBoostFillSpeed = 1f;

    [Space]

    public float fillTimeSpeed = 1f;
    public float timeToFillInSeconds = 10;

    public float TimeToFill { get => timeToFillInSeconds; }

    public Dictionary<string, UpgradeDatabase.Upgrade> upgrades;
    
    public Action<bool> OnHasProfessorChanged;

    private TappableSchool tappable;
    public TappableSchool Tappable {  get { return tappable; } }
    private SchoolVisual visual;
    public SchoolVisual Visual { get { return visual; } }

    private void Awake()
    {
        tappable = GetComponent<TappableSchool>();
        visual = GetComponent<SchoolVisual>();
    }

    private IEnumerator Start()
    {
        upgrades = UpgradeDatabase.Instance.GetUpgrades();
        yield return new WaitForEndOfFrame();

        if (!isUnlocked) yield break;

        visual.SetCostText(false);
        visual.SetProgressionSlider(true);
        upgrades["Students"].currentQuantity = 1;
        isUnlocked = true;

        SchoolsManager.Instance.boughtSchools.Add(this);
    }
}

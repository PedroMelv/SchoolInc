using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SchoolData : MonoBehaviour
{
    public bool isUnlocked;
    [Space]

    public int initialCost = 1;
    public int studentsCount = 1;
    public int initialRevenue = 1;
    public float incomeMultiplier = 1;
    public ulong base_maxMoneyHold = 500;
    public BigInteger maxMoneyHold;

    [Space]
    [Range(0f, 1f)] public float tapBoostStrength = .01f;
    public int tapBoostMax = 10;
    public float tapBoostFillSpeed = 1f;

    [Space]

    public float fillTimeSpeed = 1f;
    public float timeToFillInSeconds = 10;

    public float TimeToFill { get => timeToFillInSeconds; }

    public int currentTier = 1;
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
        maxMoneyHold += base_maxMoneyHold;
        yield return new WaitForEndOfFrame();

        if (!isUnlocked) yield break;

        visual.SetCostText(false);
        visual.SetProgressionSlider(true);
        isUnlocked = true;

        SchoolsManager.Instance.boughtSchools.Add(this);
    }

    public UpgradeDatabase.Upgrade GetUpgrade(string upgradeName)
    {
        bool got = upgrades.TryGetValue(upgradeName, out UpgradeDatabase.Upgrade upgrade);
        return upgrade;
    }

    public bool IsTierCompleted(int tier)
    {
        bool result = false;
        tier -= 1;

        int amount = 0;

        for (int i = 0; i < UpgradeDatabase.Instance.tiers[tier].upgrades.Length; i++)
        {
            bool got = upgrades.TryGetValue(UpgradeDatabase.Instance.tiers[tier].upgrades[i].nameID, out UpgradeDatabase.Upgrade upgrade);

            if(got)
            {
                amount += upgrade.currentQuantity;
            }
        }

        if (amount == 6) result = true;

        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public enum UpgradeType
{
    REVENUE,
    INCOME,
    STUDENT,
    TEACHER,
    SLIDER_SPEED,
    HOLDING_INCREASE,
    TAPBOOST_AMOUNT,
    TAPBOOST_STRENGTH,
    TAPBOOST_SPEED
}

public enum IncreaseType
{
    FLAT,
    PERCENTAGE,
    MULTIPLY
}

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "UpgradeDatabase")]
public class UpgradeDatabase : ScriptableObject
{
    public Upgrades[] tiers;

    public Upgrades[] GetUpgrades()
    {
        Upgrades[] ups = new Upgrades[tiers.Length];

        for (int i = 0; i < ups.Length; i++)
        {
            ups[i] = new Upgrades(tiers[i]);
        }

        return ups;
    }

    public int GetTierFromUpgrade(string upgradeID)
    {
        for (int i = 0; i < tiers.Length; i++)
        {
            for (int l = 0; l < tiers[i].upgrades.Length; l++)
            {
                if (tiers[i].upgrades[l].nameID == upgradeID) return i + 1;
            }
        }

        return 0;
    }

    [System.Serializable]
    public class Upgrades
    {
        public int upgradesBrought;
        public Upgrade[] upgrades;
        public Upgrade prize;
        public bool prizeGot;

        public Upgrades(Upgrades copy)
        {
            upgradesBrought = copy.upgradesBrought;
            upgrades = new Upgrade[copy.upgrades.Length];
            for (int i = 0; i < upgrades.Length; i++)
            {
                upgrades[i] = new Upgrade(copy.upgrades[i]);
            }
            prize = copy.prize;
            prizeGot = copy.prizeGot;
        }
    }

    [System.Serializable]
    public class Upgrade
    {
        public string name = "New Upgrade";
        public string nameID = "UPGRADE";
        public int maxQuantity = 100;
        public double costBase = 1;
        [Range(1f,1.5f)]public float costGrowth = 1f;

        [Space]
        public UpgradeType upgradeType;
        public IncreaseType increaseType;
        public float increaseValue;
        public bool triggerAutomatic;

        public BigInteger Price => new BigInteger(costBase * Mathf.Pow(costGrowth, currentQuantity));

        [HideInInspector] public int currentQuantity = 0;

        public Upgrade(Upgrade copy)
        {
            name = copy.name;
            nameID = copy.nameID;
            maxQuantity = copy.maxQuantity;
            costBase = copy.costBase;
            costGrowth = copy.costGrowth;
            upgradeType = copy.upgradeType;
            increaseType = copy.increaseType;
            increaseValue = copy.increaseValue;
            triggerAutomatic = copy.triggerAutomatic;
            currentQuantity = 0;
        }
    }
}

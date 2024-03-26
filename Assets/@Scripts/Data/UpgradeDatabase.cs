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

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "StaticScriptableObjects/UpgradeDatabase")]
public class UpgradeDatabase : SingletonObject<UpgradeDatabase>
{
    public Upgrades[] tiers;

    public Dictionary<string, Upgrade> GetUpgrades()
    {
        Dictionary<string, Upgrade> dictionary = new Dictionary<string, Upgrade>();

        for (int i = 0; i < tiers.Length; i++)
        {
            for (int l = 0; l < tiers[i].upgrades.Length; l++)
            {
                dictionary.Add(tiers[i].upgrades[l].nameID, new Upgrade(tiers[i].upgrades[l]));
            }
        }

        

        return dictionary;
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

        public BigInteger Price 
        { 
            get 
            {
                return new BigInteger(costBase * Mathf.Pow(costGrowth, currentQuantity));
            } 
        }

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

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "StaticScriptableObjects/UpgradeDatabase")]
public class UpgradeDatabase : SingletonObject<UpgradeDatabase>
{
    public Upgrades upgrades;

    public Dictionary<string, Upgrade> GetUpgrades()
    {
        Dictionary<string, Upgrade> dictionary = new Dictionary<string, Upgrade>();

        for (int i = 0; i < upgrades.upgrades.Length; i++)
        {
            dictionary.Add(upgrades.upgrades[i].name, new Upgrade(upgrades.upgrades[i]));
        }

        return dictionary;
    }

    [System.Serializable]
    public class Upgrades
    {
        public Upgrade[] upgrades;

        

    }

    [System.Serializable]
    public class Upgrade
    {
        public string name = "New Upgrade";
        public int maxQuantity = 100;
        public double costBase = 1;
        [Range(1f,1.5f)]public float costGrowth = 1f;
        [Range(0f,1f)]public float multiplierGrowth = 1f;

        [Space]
        public bool triggerAutomatic;

        public BigInteger Price 
        { 
            get 
            {
                return new BigInteger(costBase * Mathf.Pow(costGrowth, currentQuantity));
            } 
        }

        public UpgradeMustHave[] mustHave;

        [HideInInspector] public float multiplier { get {  return Mathf.Pow(multiplierGrowth, currentQuantity); } }
        [HideInInspector] public int currentQuantity = 0;

        public Upgrade(Upgrade copy)
        {
            name = copy.name;
            maxQuantity = copy.maxQuantity;
            costBase = copy.costBase;
            costGrowth = copy.costGrowth;
            mustHave = copy.mustHave;
            multiplierGrowth = copy.multiplierGrowth;
            triggerAutomatic = copy.triggerAutomatic;
            currentQuantity = 0;
        }
    }

    [System.Serializable]
    public class UpgradeMustHave
    {
        public string name;
        public int amount;
    }
}

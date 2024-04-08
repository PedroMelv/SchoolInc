using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class AscendedHandler : Singleton<AscendedHandler>, IBind<AscendedHandler.AscendedUpgradeData>
{
    public UpgradeDatabase upgradeDatabase;
    [SerializeField] private AscendedStoreUI storeUI;

    [Space]

    public float revenueMultiplier;
    public float incomeMultiplier;
    public int studentsStartCount;
    public float fillTimeSpeedReducer;
    public ulong maxMoneyHoldIncrease;

    private UpgradeDatabase.Upgrades[] upgrades;

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private AscendedUpgradeData data;

    private void Start()
    {
        if (upgrades == null || upgrades.Length == 0)
        {
            upgrades = upgradeDatabase.GetUpgrades();
        }
    }

    public void Ascend()
    {
        storeUI.InitializeArea();
    }

    public void CloseStore()
    {
        data.revenueMultiplier = revenueMultiplier;
        data.incomeMultiplier = incomeMultiplier;
        data.studentsStartCount = studentsStartCount;
        data.fillTimeSpeedReducer = fillTimeSpeedReducer;
        data.maxMoneyHoldIncrease = maxMoneyHoldIncrease;

        SaveLoadSystem.Instance.ResetAscendGame();

        SaveLoadSystem.Instance.SaveGame();

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public UpgradeDatabase.Upgrade GetUpgrade(string upgradeName)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            for (int l = 0; l < upgrades[i].upgrades.Length; l++)
            {
                if (upgrades[i].upgrades[l].nameID == upgradeName)
                {
                    return upgrades[i].upgrades[l];
                }
            }
        }

        return null;
    }

    public void OnBuy(UpgradeDatabase.Upgrade upgrade)
    {
        upgrade.currentQuantity++;

        switch (upgrade.upgradeType)
        {
            case UpgradeType.REVENUE:
                revenueMultiplier = HandleIncrease(revenueMultiplier, upgrade);
                break;
            case UpgradeType.INCOME:
                incomeMultiplier = HandleIncrease(incomeMultiplier, upgrade);
                break;
            case UpgradeType.STUDENT:
                studentsStartCount = HandleIncrease(studentsStartCount, upgrade);
                break;
            case UpgradeType.TEACHER:
                //fillTimeSpeed = HandleIncrease(fillTimeSpeed, upgrade);
                break;
            case UpgradeType.SLIDER_SPEED:
                fillTimeSpeedReducer = HandleIncrease(fillTimeSpeedReducer, upgrade);
                break;
            case UpgradeType.HOLDING_INCREASE:
                maxMoneyHoldIncrease = HandleIncrease(maxMoneyHoldIncrease, upgrade);
                break;
            case UpgradeType.TAPBOOST_AMOUNT:
                //tapBoostMax = HandleIncrease(tapBoostMax, upgrade);
                break;
            case UpgradeType.TAPBOOST_STRENGTH:
                //tapBoostStrength = HandleIncrease(tapBoostStrength, upgrade);
                break;
            case UpgradeType.TAPBOOST_SPEED:
                //tapBoostFillSpeed = HandleIncrease(tapBoostFillSpeed, upgrade);
                break;
            default:
                break;
        }

        data.savedUpgrades = new UpgradeSave[upgrades.Length];

        for (int i = 0; i < data.savedUpgrades.Length; i++)
        {
            data.savedUpgrades[i] = new UpgradeSave(new int[upgrades[i].upgrades.Length]);
            for (int j = 0; j < data.savedUpgrades[i].purchasedAmount.Length; j++)
            {
                data.savedUpgrades[i].purchasedAmount[j] = upgrades[i].upgrades[j].currentQuantity;
            }
        }
    }

    private BigInteger HandleIncrease(BigInteger value, UpgradeDatabase.Upgrade upgrade)
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

    private int HandleIncrease(int value, UpgradeDatabase.Upgrade upgrade)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return Mathf.RoundToInt(value + upgrade.increaseValue);
            case IncreaseType.PERCENTAGE:
                return Mathf.RoundToInt(value * (1 + upgrade.increaseValue / 100f));
            case IncreaseType.MULTIPLY:
                return Mathf.RoundToInt(value * upgrade.increaseValue);
        }
        return value;
    }

    private ulong HandleIncrease(ulong value, UpgradeDatabase.Upgrade upgrade)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return value + (ulong)upgrade.increaseValue;
            case IncreaseType.PERCENTAGE:
                return (ulong)(value * (1 + (upgrade.increaseValue / 100f)));
            case IncreaseType.MULTIPLY:
                return (ulong)(value * upgrade.increaseValue);
        }
        return value;
    }
    private float HandleIncrease(float value, UpgradeDatabase.Upgrade upgrade)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return value + upgrade.increaseValue;
            case IncreaseType.PERCENTAGE:
                return value * (1 + (upgrade.increaseValue / 100f));
            case IncreaseType.MULTIPLY:
                return value * upgrade.increaseValue;
        }
        return value;
    }

    private double HandleIncrease(double value, UpgradeDatabase.Upgrade upgrade)
    {
        switch (upgrade.increaseType)
        {
            case IncreaseType.FLAT:
                return value + upgrade.increaseValue;
            case IncreaseType.PERCENTAGE:
                return value * (1 + (upgrade.increaseValue / 100f));
            case IncreaseType.MULTIPLY:
                return value * upgrade.increaseValue;
        }
        return value;
    }

    public void Bind(AscendedUpgradeData data)
    {
        this.data = data;
        this.data.Id = Id;

        revenueMultiplier = data.revenueMultiplier == 0 ? 1 : data.revenueMultiplier;
        incomeMultiplier = data.incomeMultiplier == 0 ? 1 : data.incomeMultiplier;
        studentsStartCount = data.studentsStartCount;
        fillTimeSpeedReducer = data.fillTimeSpeedReducer == 0 ? 1 : data.fillTimeSpeedReducer;
        maxMoneyHoldIncrease = data.maxMoneyHoldIncrease == 0 ? 1 : data.maxMoneyHoldIncrease;

    }

    [System.Serializable]
    public class AscendedUpgradeData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public UpgradeSave[] savedUpgrades;

        public float revenueMultiplier;
        public float incomeMultiplier;
        public int studentsStartCount;
        public float fillTimeSpeedReducer;
        public ulong maxMoneyHoldIncrease;

        public void Reset()
        {
            savedUpgrades = null;
        }

        public void Reset_Ascended()
        {
            savedUpgrades = null;
        }
    }
}

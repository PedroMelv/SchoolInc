using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SchoolData : MonoBehaviour, IBind<SchoolData.SchoolDataSave>
{
    [SerializeField] private SchoolsManager schoolsManager;
    public SchoolsManager SchoolsManager { get => schoolsManager; }
    [SerializeField] private UpgradeDatabase upgradeDatabase;
    public UpgradeDatabase UpgradeDatabase { get => upgradeDatabase; }
    public bool isUnlocked;
    [Space]

    public double initialCost = 1;
    public int studentsCount = 1;
    public int initialRevenue = 1;
    public float priceScaling = 1f;
    public float incomeMultiplier = 1;
    public ulong base_maxMoneyHold = 500;
    public BigInteger maxMoneyHold;
    public BigInteger holdingMoney;

    public bool isAutomatic = false;

    public bool IsAutomatic { get => isAutomatic;
        set
        {
            isAutomatic = value;
            if (tappable == null)
            {
                tappable = GetComponent<TappableSchool>();
            }
            tappable.Automatic.SetInfinity(isAutomatic);
        }
    }

    [Space]
    [Range(0f, 1f)] public float tapBoostStrength = .01f;
    public int tapBoostMax = 10;
    public float tapBoostFillSpeed = 1f;

    [Space]

    public float fillTimeSpeed = 1f;
    public float timeToFillInSeconds = 10;

    public float TimeToFill { get => timeToFillInSeconds; }

    public int currentTier = 1;
    public UpgradeDatabase.Upgrades[] upgrades;
    
    public Action<bool> OnHasProfessorChanged;

    private TappableSchool tappable;
    public TappableSchool Tappable {  get { return tappable; } }
    private SchoolVisual visual;
    public SchoolVisual Visual { get { return visual; } }

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [SerializeField] private SchoolDataSave data;

    private void Awake()
    {
        tappable = GetComponent<TappableSchool>();
        visual = GetComponent<SchoolVisual>();
    }

    private IEnumerator Start()
    {
        if (upgrades == null || upgrades.Length == 0)
        {
            upgrades = upgradeDatabase.GetUpgrades();
        }
        if(maxMoneyHold <= 0)maxMoneyHold += base_maxMoneyHold;
        yield return new WaitForEndOfFrame();

        if (!isUnlocked) yield break;

        visual.SetCostText(false);
        visual.SetProgressionSlider(true);
        if(holdingMoney > 0)visual.SetCollectButtonActive(true, MoneyUtils.MoneyString(holdingMoney, "$"));

        isUnlocked = true;
        schoolsManager.boughtSchools.Add(this);

    }

    private void Update()
    {
        data.currentTier = currentTier;
        data.isAutomatic = isAutomatic;
        data.isUnlocked = isUnlocked;
        data.studentsCount = studentsCount;
        data.initialRevenue = initialRevenue;
        data.incomeMultiplier = incomeMultiplier;
        data.maxMoneyHold = maxMoneyHold.ToString();
        data.holdingMoney = holdingMoney.ToString();
        data.tapBoostStrength = tapBoostStrength;
        data.tapBoostMax = tapBoostMax;
        data.tapBoostFillSpeed = tapBoostFillSpeed;
        data.fillTimeSpeed = fillTimeSpeed;
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

    public bool IsTierCompleted(int tier)
    {
        bool result = false;
        tier -= 1;

        int amount = 0;

        for (int i = 0; i < upgradeDatabase.tiers[tier].upgrades.Length; i++)
        {
            UpgradeDatabase.Upgrade upgrade = GetUpgrade(upgradeDatabase.tiers[tier].upgrades[i].nameID);

            if(upgrade != null)
            {
                amount += upgrade.currentQuantity;
            }
        }

        if (amount == 6)
        {
            result = true;


            if (upgrades[tier].prizeGot == false)
            {
                OnBuy(upgrades[tier].prize);
                upgrades[tier].prizeGot = true;
            }
        }

        return result;
    }

    public void OnBuy(UpgradeDatabase.Upgrade upgrade)
    {
        upgrade.currentQuantity++;

        if (upgrade.triggerAutomatic)
            IsAutomatic = true;


        switch (upgrade.upgradeType)
        {
            case UpgradeType.REVENUE:
                initialRevenue = HandleIncrease(initialRevenue, upgrade);
                break;
            case UpgradeType.INCOME:
                incomeMultiplier = HandleIncrease(incomeMultiplier, upgrade);
                break;
            case UpgradeType.STUDENT:
                studentsCount = HandleIncrease(studentsCount, upgrade);
                break;
            case UpgradeType.TEACHER:
                fillTimeSpeed = HandleIncrease(fillTimeSpeed, upgrade);
                break;
            case UpgradeType.SLIDER_SPEED:
                fillTimeSpeed = HandleIncrease(fillTimeSpeed, upgrade);
                break;
            case UpgradeType.HOLDING_INCREASE:
                maxMoneyHold = HandleIncrease(maxMoneyHold, upgrade);
                break;
            case UpgradeType.TAPBOOST_AMOUNT:
                tapBoostMax = HandleIncrease(tapBoostMax, upgrade);
                break;
            case UpgradeType.TAPBOOST_STRENGTH:
                tapBoostStrength = HandleIncrease(tapBoostStrength, upgrade);
                break;
            case UpgradeType.TAPBOOST_SPEED:
                tapBoostFillSpeed = HandleIncrease(tapBoostFillSpeed, upgrade);
                break;
            default:
                break;
        }

        data.savedUpgrades = new SchoolDataSave.UpgradeSave[upgrades.Length];

        for (int i = 0; i < data.savedUpgrades.Length; i++)
        {
            data.savedUpgrades[i] = new SchoolDataSave.UpgradeSave(new int[upgrades[i].upgrades.Length]);
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

    public void Bind(SchoolDataSave data)
    {
        this.data = data;
        this.data.Id = Id;

        if(isUnlocked == false)
            isUnlocked = data.isUnlocked;
        studentsCount = data.studentsCount != 0 ? data.studentsCount : studentsCount;
        initialRevenue = data.initialRevenue != 0 ? data.initialRevenue : initialRevenue;
        incomeMultiplier = data.incomeMultiplier != 0 ? data.incomeMultiplier : incomeMultiplier;
        IsAutomatic = data.isAutomatic;
        if(BigInteger.TryParse(data.maxMoneyHold, out BigInteger moneyHold))
        {
            maxMoneyHold = moneyHold;
        }
        else
        {
            maxMoneyHold = base_maxMoneyHold;
        }

        if (BigInteger.TryParse(data.holdingMoney, out moneyHold))
        {
            holdingMoney = moneyHold;
        }
        else
        {
            holdingMoney = 0;
        }

        tapBoostStrength = data.tapBoostStrength != 0 ? data.tapBoostStrength : tapBoostStrength;
        tapBoostMax = data.tapBoostMax != 0 ? data.tapBoostMax : tapBoostMax;
        tapBoostFillSpeed = data.tapBoostFillSpeed != 0 ? data.tapBoostFillSpeed : tapBoostFillSpeed;
        fillTimeSpeed = data.fillTimeSpeed != 0 ? data.fillTimeSpeed : fillTimeSpeed;
        currentTier = data.currentTier != 0 ? data.currentTier : currentTier;

        if (data.savedUpgrades != null)
        {
            
            upgrades = upgradeDatabase.GetUpgrades();
            
            for (int l = 0; l < upgrades.Length; l++)
            {
                upgrades[l].upgradesBrought = data.savedUpgrades[l].TotalPurchased();
                for (int y = 0; y < upgrades[l].upgrades.Length; y++)
                {
                    upgrades[l].upgrades[y].currentQuantity = data.savedUpgrades[l].purchasedAmount[y];
                }
            }
            
        }
    }

    [Serializable]
    public class SchoolDataSave : ISaveable
    {
        //TODO: TapBoost está resetando toda vez que o jogo fecha e abre, precisa trackear quantos ele tinha quando fechou e quantos ele recuperou enquanto o app estava fechado
        [field: SerializeField]public SerializableGuid Id { get; set; }

        public bool isUnlocked;

        public int studentsCount;
        public int initialRevenue;
        public bool isAutomatic;
        public float incomeMultiplier;

        public string maxMoneyHold;
        public string holdingMoney;

        public float tapBoostStrength;
        public int tapBoostMax;

        public float tapBoostFillSpeed;

        public float fillTimeSpeed;

        public int currentTier;

        public UpgradeSave[] savedUpgrades;

        [Serializable]
        public struct UpgradeSave
        {
            public int[] purchasedAmount;

            public UpgradeSave(int[] purchaseAmount)
            {
                this.purchasedAmount = purchaseAmount;
            }

            public int TotalPurchased()
            {
                int total = 0;
                for (int i = 0; i < purchasedAmount.Length; i++)
                {
                    total += purchasedAmount[i];
                }

                return total;
            }
        }

        public void Reset()
        {
            isUnlocked = false;
            studentsCount = 0;
            initialRevenue = 0;
            isAutomatic = false;
            incomeMultiplier = 0;
            maxMoneyHold = "reseted";
            holdingMoney = "reseted";
            tapBoostStrength = 0;
            tapBoostMax = 0;
            tapBoostFillSpeed = 0;
            fillTimeSpeed = 0;
            currentTier = 0;
            savedUpgrades = null;
        }
    }
}

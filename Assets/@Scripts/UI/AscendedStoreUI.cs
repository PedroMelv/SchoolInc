using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AscendedStoreUI : StoreUI<AscendedStoreUI>
{
    [SerializeField] private TextMeshProUGUI ascendedCoinsText;
    public override void InitializeArea()
    {
        base.InitializeArea();

        ascendedCoinsText.text = GameCurrency.Instance.CoinCurrencyString;

        UpdateStoreContainer();
    }

    protected override void InitializeBuyNodes()
    {
        AscendedHandler handler = FindObjectOfType<AscendedHandler>();
        UpgradeDatabase.Upgrades[] tiers = handler.upgradeDatabase.tiers;

        for (int i = 0; i < tiers.Length; i++)
        {
            //Instantiate(tierPrefab, storeContainer).text = "Tier " + (i + 1);

            for (int l = 0; l < tiers[i].upgrades.Length; l++)
            {
                Debug.Log("Instancing");
                nodesOnScreen.Add(Instantiate(buyPrefab, storeContainer));
                nodesOnScreen[l + i * 3].Initialize(handler.GetUpgrade(tiers[i].upgrades[l].nameID), 1f);


                nodesOnScreen[l + i * 3].onButtonClickCallback += UpdateBuyNodes;
                nodesOnScreen[l + i * 3].interactable = true;
            }
        }
    }

    protected override void UpdateBuyNodes()
    {
        AscendedHandler handler = FindObjectOfType<AscendedHandler>();
        for (int i = 0; i < nodesOnScreen.Count; i++)
        {
            nodesOnScreen[i].Initialize(handler.GetUpgrade(nodesOnScreen[i].Upgrade.nameID), 1f);
        }
        
        ascendedCoinsText.text = GameCurrency.Instance.CoinCurrencyString;
    }

    public override void UpdateStoreContainer()
    {
        base.UpdateStoreContainer();
    }
}
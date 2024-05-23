using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AscendedStoreUI : StoreUI<AscendedStoreUI>
{
    [SerializeField] private GameObject ascendedWarning;
    [SerializeField] private TextMeshProUGUI ascendedCoinsText;
    public override void InitializeArea()
    {
        ascendedWarning.SetActive(true);

        
    }

    public void ConfirmWarning()
    {
        ascendedWarning.SetActive(false);
        storeObject.SetActive(true);

        ascendedCoinsText.text = GameCurrency.Instance.CoinCurrencyString;

        UpdateStoreContainer();
    }

    public void CancelWarning()
    {
        ascendedWarning.SetActive(false);
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
                BuyNode buyNode = Instantiate(buyPrefab, storeContainer);
                buyNode.suffix = "L$";
                nodesOnScreen.Add(buyNode);
                nodesOnScreen[l + i * 3].Initialize(handler.GetUpgrade(tiers[i].upgrades[l].nameID), 1f);

                int indexA = l;
                int indexB = i;

                nodesOnScreen[l + i * 3].onButtonClickCallback += () => OnBuy(nodesOnScreen[indexA + indexB * 3]);
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
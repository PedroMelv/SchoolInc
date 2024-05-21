using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscendedBuyNode : BuyNode
{
    protected override void InitializeOnBuyEvent()
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => GameCurrency.Instance.RemoveCurrencyAscended((double)upgrade.Price * scaling, null, OnBuy));
        buyButton.onClick.AddListener(() => onButtonClickCallback?.Invoke());
    }

    protected override void OnBuy()
    {
        AscendedHandler data = FindObjectOfType<AscendedHandler>();

        data.OnBuy(upgrade);

        //SaveLoadSystem.Instance.SaveGame();
    }
}

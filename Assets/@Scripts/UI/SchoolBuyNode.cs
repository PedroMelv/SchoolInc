using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBuyNode : BuyNode
{

    protected override void OnBuy()
    {
        SchoolData data = SchoolsManager.Instance.SchoolSelected;

        data.OnBuy(upgrade);

        //SaveLoadSystem.Instance.SaveGame();
    }
}

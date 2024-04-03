using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SchoolAreaStore : StoreUI<SchoolAreaStore>
{
    private int schoolSelected = 0;
    private int maxSchools = 0;

    private SchoolData currentSchool;

    public override void InitializeArea()
    {
        maxSchools = SchoolsManager.Instance.boughtSchools.Count;
        schoolSelected = SchoolsManager.Instance.schoolSelectedIndex;

        inputHandler.cameraInput.ChangeState(new CameraInput.CameraState_StoreFocused());

        currentSchool = SchoolsManager.Instance.SchoolSelected;

        base.InitializeArea();
    }
    public override void CloseArea()
    {
        currentSchool = null;
        inputHandler.cameraInput.ChangeState(new CameraInput.CameraState_InputHandled());
    }
    public override void UpdateStoreContainer()
    {
        if (schoolSelected == -1 || currentSchool == null) return;

        currentSchool = currentSchool.SchoolsManager.SchoolSelected;

        base.UpdateStoreContainer();
    }

    protected override void InitializeBuyNodes()
    {
        UpgradeDatabase.Upgrades[] tiers = currentSchool.UpgradeDatabase.tiers;

        for (int i = 0; i < tiers.Length; i++)
        {
            Instantiate(tierPrefab, storeContainer).text = "Tier " + (i+1);

            bool isCurrentTier = (i + 1) == currentSchool.currentTier;


            for (int l = 0; l < tiers[i].upgrades.Length; l++)
            {
                nodesOnScreen.Add(Instantiate(buyPrefab, storeContainer));
                nodesOnScreen[l + i * 3].Initialize(currentSchool.GetUpgrade(tiers[i].upgrades[l].nameID), currentSchool.priceScaling);

                if (tiers[i].upgrades[l].currentQuantity == tiers[i].upgrades[l].maxQuantity) 
                    isCurrentTier = false;


                nodesOnScreen[l + i * 3].onButtonClickCallback += UpdateBuyNodes;
                nodesOnScreen[l + i * 3].interactable = isCurrentTier;
            }
        }
    }
    protected override void UpdateBuyNodes()
    {
        for (int i = 0; i < nodesOnScreen.Count; i++)
        {
            bool isCurrentTier = currentSchool.UpgradeDatabase.GetTierFromUpgrade(nodesOnScreen[i].Upgrade.nameID) == currentSchool.currentTier;
            
            if(isCurrentTier && currentSchool.IsTierCompleted(currentSchool.currentTier))
            {
                currentSchool.currentTier++;
                isCurrentTier = currentSchool.UpgradeDatabase.GetTierFromUpgrade(nodesOnScreen[i].Upgrade.nameID) == currentSchool.currentTier;
            }
            nodesOnScreen[i].Initialize(currentSchool.GetUpgrade(nodesOnScreen[i].Upgrade.nameID), currentSchool.priceScaling);

            if (nodesOnScreen[i].Upgrade.currentQuantity == nodesOnScreen[i].Upgrade.maxQuantity) 
                isCurrentTier = false;
                
            nodesOnScreen[i].interactable = isCurrentTier;
        
        }
    }

    public void ChangeSchoolSelected(int amount)
    {
        //if (schoolSelected == -1) return;
        schoolSelected += amount;

        if (schoolSelected < 0) schoolSelected = maxSchools - 1;
        if (schoolSelected >= maxSchools) schoolSelected = 0;

        currentSchool.SchoolsManager.SetSelected(schoolSelected);

        UpdateStoreContainer();
    }
}

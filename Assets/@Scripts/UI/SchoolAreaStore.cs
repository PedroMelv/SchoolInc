using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SchoolAreaStore : MonoBehaviour
{
    private int schoolSelected = 0;
    private int maxSchools = 0;

    [SerializeField] private Transform storeContainer;
    [SerializeField] private TextMeshProUGUI tierPrefab;
    [SerializeField] private BuyNode buyPrefab;

    private List<BuyNode> nodesOnScreen = new List<BuyNode>();

    private SchoolData currentSchool;

    public void InitializeArea()
    {
        maxSchools = SchoolsManager.Instance.boughtSchools.Count;
        schoolSelected = SchoolsManager.Instance.schoolSelectedIndex;

        InputHandler.Instance.cameraInput.ChangeState(new CameraInput.CameraState_StoreFocused());

        UpdateVisualSchool();
    }

    public void CloseArea()
    {
        currentSchool = null;
        InputHandler.Instance.cameraInput.ChangeState(new CameraInput.CameraState_InputHandled());
    }

    public void UpdateVisualSchool()
    {
        if (schoolSelected == -1) return;

        currentSchool = SchoolsManager.Instance.SchoolSelected;

        if (nodesOnScreen.Count > 0)
        {
            UpdateBuyNodes();
            return;
        }

        InitializeBuyNodes();
    }

    private void InitializeBuyNodes()
    {
        UpgradeDatabase.Upgrades[] tiers = UpgradeDatabase.Instance.tiers;

        for (int i = 0; i < tiers.Length; i++)
        {
            Instantiate(tierPrefab, storeContainer).text = "Tier " + (i+1);

            bool isCurrentTier = (i + 1) == currentSchool.currentTier;

            for (int l = 0; l < tiers[i].upgrades.Length; l++)
            {
                nodesOnScreen.Add(Instantiate(buyPrefab, storeContainer));
                nodesOnScreen[l + i * 3].Initialize(currentSchool.GetUpgrade(tiers[i].upgrades[l].nameID));

                nodesOnScreen[l + i * 3].onButtonClickCallback += UpdateBuyNodes;
                nodesOnScreen[l + i * 3].interactable = isCurrentTier;
            }
        }
    }
    private void UpdateBuyNodes()
    {
        for (int i = 0; i < nodesOnScreen.Count; i++)
        {
            bool isCurrentTier = UpgradeDatabase.Instance.GetTierFromUpgrade(nodesOnScreen[i].Upgrade.nameID) == currentSchool.currentTier;
            if(isCurrentTier && currentSchool.IsTierCompleted(currentSchool.currentTier))
            {
                currentSchool.currentTier++;
                isCurrentTier = UpgradeDatabase.Instance.GetTierFromUpgrade(nodesOnScreen[i].Upgrade.nameID) == currentSchool.currentTier;
            }
            nodesOnScreen[i].Initialize(currentSchool.GetUpgrade(nodesOnScreen[i].Upgrade.nameID));

                
            nodesOnScreen[i].interactable = isCurrentTier;
        
        }
    }

    public void ChangeSchoolSelected(int amount)
    {
        //if (schoolSelected == -1) return;
        schoolSelected += amount;

        if (schoolSelected < 0) schoolSelected = maxSchools - 1;
        if (schoolSelected >= maxSchools) schoolSelected = 0;

        SchoolsManager.Instance.SetSelected(schoolSelected);

        UpdateVisualSchool();
    }
}

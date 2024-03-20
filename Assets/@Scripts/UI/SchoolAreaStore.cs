using System.Collections.Generic;
using UnityEngine;

public class SchoolAreaStore : MonoBehaviour
{
    private int schoolSelected = 0;
    private int maxSchools = 0;

    [SerializeField] private Transform storeContainer;
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
        UpgradeDatabase.Upgrades allUpgrades = UpgradeDatabase.Instance.upgrades;

        for (int i = 0; i < allUpgrades.upgrades.Length; i++)
        {
            UpgradeDatabase.Upgrade upgrade = null;

            if (!currentSchool.upgrades.TryGetValue(allUpgrades.upgrades[i].name, out upgrade))
            {
                continue;
            }

            bool nodeIsActive = true;

            if (upgrade.mustHave.Length > 0)
            {
                bool mustHaveIsCompleted = false;
                for (int l = 0; l < upgrade.mustHave.Length; l++)
                {
                    UpgradeDatabase.Upgrade upgradeMustHave = null;
                    if (currentSchool.upgrades.TryGetValue(upgrade.mustHave[l].name, out upgradeMustHave))
                        if (upgradeMustHave.currentQuantity >= upgrade.mustHave[l].amount)
                        {
                            mustHaveIsCompleted = true;
                        }
                        else
                        {
                            mustHaveIsCompleted = false;
                            break;
                        }
                }

                if (!mustHaveIsCompleted) nodeIsActive = false;
            }

            BuyNode node = Instantiate(buyPrefab, storeContainer);

            node.onButtonClickCallback += UpdateBuyNodes;
            node.Initialize(upgrade);
            nodesOnScreen.Add(node);

            node.gameObject.SetActive(nodeIsActive);
        }
    }
    private void UpdateBuyNodes()
    {
        UpgradeDatabase.Upgrades allUpgrades = UpgradeDatabase.Instance.upgrades;

        for (int i = 0; i < allUpgrades.upgrades.Length; i++)
        {
            UpgradeDatabase.Upgrade upgrade = null;

            if (!currentSchool.upgrades.TryGetValue(allUpgrades.upgrades[i].name, out upgrade))
            {
                continue;
            }

            bool nodeIsActive = true;

            if (upgrade.mustHave.Length > 0)
            {
                bool mustHaveIsCompleted = false;
                for (int l = 0; l < upgrade.mustHave.Length; l++)
                {
                    UpgradeDatabase.Upgrade upgradeMustHave = null;
                    if (currentSchool.upgrades.TryGetValue(upgrade.mustHave[l].name, out upgradeMustHave))
                        if (upgradeMustHave.currentQuantity >= upgrade.mustHave[l].amount)
                        {
                            mustHaveIsCompleted = true;
                        }
                        else
                        {
                            mustHaveIsCompleted = false;
                            break;
                        }
                }

                if (!mustHaveIsCompleted) nodeIsActive = false;
            }

            nodesOnScreen[i].Initialize(upgrade);
            nodesOnScreen[i].gameObject.SetActive(nodeIsActive);
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

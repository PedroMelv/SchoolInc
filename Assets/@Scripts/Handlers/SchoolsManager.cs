using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolsManager : StaticInstance<SchoolsManager>
{
    public List<SchoolData> boughtSchools = new List<SchoolData>();

    private SchoolData schoolSelected;
    public SchoolData SchoolSelected
    {
        get => schoolSelected;

        set
        {
            if (schoolSelected != null) schoolSelected.Tappable.OnTapCountChanged -= TapCountChanged;
            
            schoolSelected = value;
            UpdateTapButton();

            if (schoolSelected == null) return;
            schoolSelected.Tappable.OnTapCountChanged += TapCountChanged;

            schoolSelected.Tappable.UpdateTapCount();
        }
    }
    public int schoolSelectedIndex { get 
        {
            if (boughtSchools.Count == 0) return -1;
            return boughtSchools.IndexOf(SchoolSelected); 
        } 
    }

    [SerializeField] private Button tapButton;
    [SerializeField] private TextMeshProUGUI tapCountText;

    private void Start()
    {
        UpdateTapButton();
    }

    public void TapButton()
    {
        if (SchoolSelected == null) return;

        SchoolSelected.Tappable.Tap(true);
    }

    private void UpdateTapButton()
    {
        if (schoolSelected == null) tapCountText.text = "";
    }
    private void TapCountChanged(int tapCount, int tapCountMax)
    {
        tapCountText.text = (tapCountMax - tapCount) + "/" + tapCountMax;
    }

    public void SetSelected(int index)
    {
        if(index < 0 || index >= boughtSchools.Count)
        {
            return;
        }

        SchoolSelected = boughtSchools[index];
    }
}

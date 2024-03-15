using UnityEditor.Build;
using UnityEngine;

public class SchoolAreaStore : MonoBehaviour
{
    private int schoolSelected = 0;
    private int maxSchools = 0;

    [SerializeField] private Transform storeContainer;
    [SerializeField] private BuyNode buyPrefab;

    public void InitializeArea()
    {
        maxSchools = SchoolsManager.Instance.boughtSchools.Count;
        schoolSelected = SchoolsManager.Instance.schoolSelectedIndex;

        InputHandler.Instance.cameraInput.ChangeState(new CameraInput.CameraState_StoreFocused());

        UpdateVisualSchool();
    }

    public void CloseArea()
    {
        InputHandler.Instance.cameraInput.ChangeState(new CameraInput.CameraState_InputHandled());
    }

    public void UpdateVisualSchool()
    {
        if (schoolSelected == -1) return;

        foreach (Transform child in storeContainer)
        {
            Destroy(child.gameObject);
        }

        SchoolData data = SchoolsManager.Instance.SchoolSelected;

        string studentCount = data.studentCount + "x";
        string price = data.studentCount + "x";

        Instantiate(buyPrefab, storeContainer).Initialize
            ("Alunos",
            studentCount,
            price,
            () => data.studentCount++,
            () =>
            {
                string studentCount = data.studentCount + "x";
                string price = data.studentCount + "x";

                return (price, studentCount);
            }
            ); ;
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

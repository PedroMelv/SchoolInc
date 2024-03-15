using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolData : MonoBehaviour
{
    public bool isUnlocked;

    [Space]
    public ulong initialCost;
    public ulong initialRevenue;

    public float timeToFillInSeconds = 10;

    [Space]
    public ulong studentCount;
    [Range(1f, 1.2f)] public float growthRate;
    [Space]
    public float directorMultiplier = 1f;

    [Space]
    public bool hasProfessor;
    public float assistantMultiplier = 1f;

    public bool HasProfessor { get { return hasProfessor; } set 
        {
            hasProfessor = value;
            OnHasProfessorChanged?.Invoke(hasProfessor);
        } 
    }
    public float TimeToFill { get => timeToFillInSeconds; }


    public Action<bool> OnHasProfessorChanged;

    private TappableSchool tappable;
    public TappableSchool Tappable {  get { return tappable; } }
    private SchoolVisual visual;
    public SchoolVisual Visual { get { return visual; } }

    private void Awake()
    {
        tappable = GetComponent<TappableSchool>();
        visual = GetComponent<SchoolVisual>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        visual.SetCostText(false);
        visual.SetProgressionSlider(true);
        studentCount = 1;
        isUnlocked = true;

        SchoolsManager.Instance.boughtSchools.Add(this);
    }
}

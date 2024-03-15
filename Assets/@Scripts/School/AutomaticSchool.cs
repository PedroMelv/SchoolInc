using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticSchool : AutomaticTapper
{
    private SchoolData data;

    protected override void Awake()
    {
        base.Awake(); 
        data = GetComponent<SchoolData>();
        data.OnHasProfessorChanged += CheckProfessor;
    }

    private void CheckProfessor(bool hasProfessor)
    {
        if (hasProfessor)
        {
            SetInfinity(true);
            StartTapper();
        }
    }

}

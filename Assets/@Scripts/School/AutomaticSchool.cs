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
    }

    public override void StartTapper()
    {
        if (data == null || data.hasProfessor == false) return;
        
        ChangeTick(data.AutomaticTick);

        base.StartTapper();
    }

}

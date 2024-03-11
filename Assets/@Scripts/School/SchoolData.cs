using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolData : MonoBehaviour
{
    public int tapToFillPerStudent = 10;

    [Space]
    public int deskCount;
    public int studentCount;

    [Space]
    public bool hasProfessor;
    public float professorEfficiency = 1f;

    public int TapMax { get => tapToFillPerStudent * studentCount; }
    public float AutomaticTick 
    { 
        get{
            if (!hasProfessor) return 0f;
            return professorEfficiency;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolData : MonoBehaviour
{
    public int timeToFillInSeconds = 10;

    [Space]
    public int studentCount;

    [Space]
    public bool hasProfessor;

    public int TimeToFill { get => timeToFillInSeconds; }

}

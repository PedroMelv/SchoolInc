using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugResource : MonoBehaviour
{
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TappableSchool[] schools = FindObjectsOfType<TappableSchool>();

            if (schools.Length > 0)
            {
                SchoolsManager.Instance.SchoolSelected = schools[0].GetComponent<SchoolData>();
                schools[0].Tap();
            }
        }
    }
}

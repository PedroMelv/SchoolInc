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
                for (int i = 0; i < schools.Length; i++)
                {
                    if (!schools[i].GetComponent<SchoolData>().isUnlocked) continue;
                    SchoolsManager.Instance.SchoolSelected = schools[i].GetComponent<SchoolData>();
                    schools[i].Tap();
                }
                
            }
        }
    }
}

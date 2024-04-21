using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SidewalkPoint))]
public class SidewalkPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SidewalkPoint sidewalkPoint = (SidewalkPoint)target;

        if (GUILayout.Button("Create new Point"))
        {
            SidewalkPoint newPoint = new GameObject("SidewalkPoint").AddComponent<SidewalkPoint>();

            newPoint.transform.position = sidewalkPoint.transform.position + new Vector3(Random.value, 0, Random.value);

            newPoint.transform.position = new Vector3(newPoint.transform.position.x, 0, newPoint.transform.position.z);
            newPoint.transform.eulerAngles = sidewalkPoint.transform.eulerAngles;


            sidewalkPoint.Add(newPoint);
            newPoint.Add(sidewalkPoint);

            newPoint.transform.parent = sidewalkPoint.transform.parent;

            Selection.activeGameObject = newPoint.gameObject;
        }
    }

     
}

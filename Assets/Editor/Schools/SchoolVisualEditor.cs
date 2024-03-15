using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SchoolVisual))]
public class SchoolVisualEditor : Editor
{
    SchoolVisual visual = null;

    private void OnSceneGUI()
    {
        if(visual == null)
        {
            visual = (SchoolVisual)target;
            return;
        }

        Vector3 pos = visual.CameraOffsetPosition;
        pos += visual.transform.position;
        pos = Handles.DoPositionHandle(pos, Quaternion.identity);
        pos -= visual.transform.position;
        visual.CameraOffsetPosition = pos;
    }
}

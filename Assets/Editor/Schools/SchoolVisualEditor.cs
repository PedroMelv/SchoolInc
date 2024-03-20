using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SchoolVisual)), CanEditMultipleObjects()]
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

        EditorGUI.BeginChangeCheck();

        Vector3 cameraOffset = DrawHandleOffseted(visual.CameraOffsetPosition);
        Vector3 costOffset = DrawHandleOffseted(visual.CostTextOffset);
        Vector3 collectOffset = DrawHandleOffseted(visual.CollectMoneyOffset);
        Vector3 progressionOffset = DrawHandleOffseted(visual.ProgressionSliderOffset);
        
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(visual, "Position Changed");

            visual.CameraOffsetPosition = cameraOffset;
            visual.CostTextOffset = costOffset;
            visual.CollectMoneyOffset = collectOffset;
            visual.ProgressionSliderOffset = progressionOffset;
        }
    }

    private Vector3 DrawHandleOffseted(Vector3 position, Vector3 offset)
    {
        Vector3 pos = position;
        pos += offset;
        pos = Handles.DoPositionHandle(pos, Quaternion.identity);
        pos -= offset;

        return pos;
    }

    private Vector3 DrawHandleOffseted(Vector3 position)
    {
        Vector3 pos = position;
        pos += visual.transform.position;
        pos = Handles.DoPositionHandle(pos, Quaternion.identity);
        pos -= visual.transform.position;

        return pos;
    }
}

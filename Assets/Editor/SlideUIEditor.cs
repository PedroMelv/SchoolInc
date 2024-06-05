using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlideUI))]
public class SlideUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SlideUI slide = (SlideUI)target;
        base.OnInspectorGUI();


        if (GUILayout.Button("Set Target to Current position"))
        {
            slide.SetTargetPositionToCurrent();
        }

        if (GUILayout.Button("Set Start to Current position"))
        {
            slide.SetStartPositionToCurrent();
        }
    }

    private void OnSceneGUI()
    {
        SlideUI slide = (SlideUI)target;

        EditorGUI.BeginChangeCheck();

        Vector3 position = slide.TransformPosition(slide.TargetPosition, slide.targetPositionStyle);

        position = Handles.DoPositionHandle(position, slide.transform.rotation);

        Handles.DrawWireCube(position, Vector3.one);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(slide, "Change on TargetPosition");
            slide.TargetPosition = slide.DeTransformPosition(position, slide.targetPositionStyle);
        }
    }
}
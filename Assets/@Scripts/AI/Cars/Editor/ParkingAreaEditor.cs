using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParkingArea))]
public class ParkingAreaEditor : Editor
{
    private void OnSceneGUI()
    {
        ParkingArea area = (ParkingArea)target;

        Vector3 convertedPosition = area.transform.position + area.startParkPosition;

        convertedPosition = Handles.PositionHandle(convertedPosition, area.transform.rotation);

        area.startParkPosition = convertedPosition - area.transform.position;
    }
}

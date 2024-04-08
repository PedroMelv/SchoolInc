using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHandler : MonoBehaviour
{
    [SerializeField] private List<Vector3> _roadParts = new List<Vector3>();

    [ContextMenu("Get Childs")]
    private void GetChilds()
    {
        _roadParts.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            _roadParts.Add(transform.TransformPoint(transform.GetChild(i).localPosition));
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (_roadParts.Count <= 1) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < _roadParts.Count - 1; i++)
        {
            Gizmos.DrawLine(_roadParts[i], _roadParts[i+1]);
        }
        
    }
}

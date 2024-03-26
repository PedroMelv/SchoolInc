using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    public float width;
    public Vector3[] positions;
    public Matrix4x4 matrix;

    private void OnDrawGizmos()
    {
        if (positions.Length < 2) return;

        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector3 center = (positions[i + 1] + positions[i]) / 2;

            Vector3 direction = (positions[i + 1] - positions[i]).normalized;
            if (direction == Vector3.zero) continue;

            matrix.SetTRS(center, Quaternion.LookRotation(direction, Vector3.up), (positions[i + 1] - positions[i]) * Vector3.Distance(positions[i + 1], positions[i]));
            Gizmos.matrix = matrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireSphere(positions[i + 1], .15f);
            Gizmos.DrawWireSphere(positions[i], .15f);
        }

        
    }
}

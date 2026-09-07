using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlythroughCamera : MonoBehaviour
{
    public Transform ownPoint;
    public float limitRange = 30f;

    private void OnDrawGizmos()
    {
        if (ownPoint != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawSphere(ownPoint.position, limitRange);
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(ownPoint.position, Quaternion.identity, new Vector3(-1, -1, -1));
            Gizmos.DrawSphere(Vector3.zero, limitRange);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
            Gizmos.DrawWireSphere(ownPoint.position, limitRange);
        }
    }
}
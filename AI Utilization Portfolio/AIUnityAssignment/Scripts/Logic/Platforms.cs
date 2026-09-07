using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class Platforms : MonoBehaviour
{
    [Header("Colliders")]
    public BoxCollider col3D;
    private MeshCollider col2D;

    [Header("Settings")]
    public bool alwaysPassThroughIn2D = false;
    public float meshThickness = 1.0f;

    private float originalZDepth;
    private Coroutine triggerCheckCoroutine;
    private Mesh generatedMesh;

    private bool isCurrently2D = false;
    private float currentTargetZ;

    void Awake()
    {
        if (col3D == null) col3D = GetComponent<BoxCollider>();
        col2D = GetComponent<MeshCollider>();
        if (col2D == null)
        {
            col2D = gameObject.AddComponent<MeshCollider>();
        }

        col2D.enabled = false;
        col2D.convex = true;
    }

    void Update()
    {
        if (isCurrently2D && transform.hasChanged)
        {
            Update2DMesh();
            transform.hasChanged = false;
        }
    }

    public void SwitchTo2D(float targetZ, CharacterController playerCC)
    {
        if (col3D == null) return;

        isCurrently2D = true;
        currentTargetZ = targetZ;
        originalZDepth = col3D.transform.TransformPoint(col3D.center).z;
        col3D.enabled = false;

        Update2DMesh();
        col2D.enabled = true;

        ComponentChangingObj compObj = GetComponent<ComponentChangingObj>();
        if (compObj != null) compObj.ApplyAttributes();

        if (triggerCheckCoroutine != null)
        {
            StopCoroutine(triggerCheckCoroutine);
            triggerCheckCoroutine = null;
        }

        bool isSkillTrigger = compObj != null && compObj.HasTriggerSkill;

        if (alwaysPassThroughIn2D || isSkillTrigger)
        {
            col2D.isTrigger = true;
        }
        else
        {
            if (IsPlayerTrapped(playerCC, col2D))
            {
                col2D.isTrigger = true;
                triggerCheckCoroutine = StartCoroutine(WaitUntilPlayerExits(playerCC, compObj));
            }
            else
            {
                col2D.isTrigger = false;
            }
        }
    }

    public void SwitchTo3D()
    {
        isCurrently2D = false;

        if (col3D == null || col2D == null) return;

        if (triggerCheckCoroutine != null)
        {
            StopCoroutine(triggerCheckCoroutine);
            triggerCheckCoroutine = null;
        }

        col2D.enabled = false;
        col3D.enabled = true;

        ComponentChangingObj compObj = GetComponent<ComponentChangingObj>();
        if (compObj != null) compObj.ApplyAttributes();
    }
    private void Update2DMesh()
    {
        if (generatedMesh != null)
        {
            Destroy(generatedMesh);
        }

        generatedMesh = GenerateProjectedMesh(currentTargetZ);
        col2D.sharedMesh = generatedMesh;
    }

    public float GetOriginalZDepth()
    {
        return originalZDepth;
    }

    public float GetOutermostZ()
    {
        if (col3D == null) return transform.position.z;
        return col3D.bounds.min.z;
    }

    private IEnumerator WaitUntilPlayerExits(CharacterController playerCC, ComponentChangingObj compObj)
    {
        while (col2D != null && col2D.enabled && col2D.isTrigger)
        {
            if (compObj != null && compObj.HasTriggerSkill)
            {
                break;
            }

            if (!IsPlayerTrapped(playerCC, col2D))
            {
                col2D.isTrigger = false;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private bool IsPlayerTrapped(CharacterController playerCC, MeshCollider col)
    {
        float radius = playerCC.radius * 0.8f;
        float halfHeight = playerCC.height * 0.5f;
        Vector3 center = playerCC.transform.position + playerCC.center;

        Vector3 p1 = center + Vector3.up * (halfHeight - radius - 0.05f);
        Vector3 p2 = center - Vector3.up * (halfHeight - radius - 0.05f);

        Collider[] overlaps = Physics.OverlapCapsule(p1, p2, radius, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider c in overlaps)
        {
            if (c == col) return true;
        }
        return false;
    }

    private Mesh GenerateProjectedMesh(float targetZ)
    {
        Vector3[] worldVerts = GetBoxColliderVertices();
        List<Vector2> projectedPoints = new List<Vector2>();
        foreach (var v in worldVerts)
        {
            projectedPoints.Add(new Vector2(v.x, v.y));
        }

        List<Vector2> hull = GetConvexHull(projectedPoints);
        return CreateExtrudedMesh(hull, targetZ, meshThickness);
    }

    private Vector3[] GetBoxColliderVertices()
    {
        Vector3[] vertices = new Vector3[8];
        Vector3 center = col3D.center;
        Vector3 extents = col3D.size * 0.5f;

        int index = 0;
        for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 localPos = center + new Vector3(extents.x * x, extents.y * y, extents.z * z);
                    vertices[index++] = col3D.transform.TransformPoint(localPos); // 월드 좌표 변환
                }
        return vertices;
    }

    private List<Vector2> GetConvexHull(List<Vector2> points)
    {
        points = points.OrderBy(p => p.x).ThenBy(p => p.y).ToList();
        List<Vector2> hull = new List<Vector2>();

        foreach (var pt in points)
        {
            while (hull.Count >= 2 && CrossProduct(hull[hull.Count - 2], hull[hull.Count - 1], pt) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(pt);
        }

        int lowerCount = hull.Count;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            var pt = points[i];
            while (hull.Count > lowerCount && CrossProduct(hull[hull.Count - 2], hull[hull.Count - 1], pt) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(pt);
        }

        hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    private float CrossProduct(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }

    private Mesh CreateExtrudedMesh(List<Vector2> hull, float zCoord, float thickness)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Projected2DMesh";

        int n = hull.Count;
        Vector3[] vertices = new Vector3[n * 2];
        float halfZ = thickness / 2f;
        for (int i = 0; i < n; i++)
        {
            Vector3 worldFront = new Vector3(hull[i].x, hull[i].y, zCoord - halfZ);
            Vector3 worldBack = new Vector3(hull[i].x, hull[i].y, zCoord + halfZ);

            vertices[i] = transform.InverseTransformPoint(worldFront);
            vertices[i + n] = transform.InverseTransformPoint(worldBack);
        }

        List<int> triangles = new List<int>();

        for (int i = 1; i < n - 1; i++)
        {
            triangles.Add(0); triangles.Add(i + 1); triangles.Add(i);
        }
        for (int i = 1; i < n - 1; i++)
        {
            triangles.Add(n); triangles.Add(n + i); triangles.Add(n + i + 1);
        }

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;

            triangles.Add(i);
            triangles.Add(next + n);
            triangles.Add(next);

            triangles.Add(i);
            triangles.Add(i + n);
            triangles.Add(next + n);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
}
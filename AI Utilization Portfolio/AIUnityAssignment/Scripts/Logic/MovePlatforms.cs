using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MovePlatforms : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("이동할 목표 지점들 (순서대로 이동)")]
    public Transform[] waypoints;
    public float speed = 5f;

    [Tooltip("웨이포인트 도달 시 대기 시간")]
    public float waitTime = 1f;
    [Tooltip("마지막 웨이포인트에서 다시 처음으로 돌아갈지 여부")]
    public bool isLooping = true;

    [Header("Dynamic NavMesh Settings")]
    [Tooltip("발판 가장자리 전체에서 본토와 연결을 시도할 탐색 반경")]
    public float navLinkRange = 3.5f;

    private int targetIndex = 0;
    private Vector3 currentVelocity;
    private Vector3 lastPosition;
    private float waitTimer = 0f;

    private List<NavMeshLinkInstance> activeLinks = new List<NavMeshLinkInstance>();

    private float linkUpdateTimer = 0f;
    private const float LINK_UPDATE_INTERVAL = 0.15f;
    private const float LINK_STEP_SIZE = 1.0f; 

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
        lastPosition = transform.position;

        UpdateDynamicNavMeshLink();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            currentVelocity = Vector3.zero;
            lastPosition = transform.position;

            ProcessLinkUpdate();
            return;
        }

        Transform target = waypoints[targetIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        float step = speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= step)
        {
            transform.position = target.position;
            waitTimer = waitTime;
            targetIndex++;

            if (targetIndex >= waypoints.Length)
            {
                targetIndex = isLooping ? 0 : waypoints.Length - 1;
            }
        }
        else
        {
            transform.position += dir * step;
        }
        currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        ProcessLinkUpdate();
    }

    private void ProcessLinkUpdate()
    {

        linkUpdateTimer += Time.deltaTime;
        if (linkUpdateTimer >= LINK_UPDATE_INTERVAL)
        {
            linkUpdateTimer = 0f;
            UpdateDynamicNavMeshLink();
        }
    }
    private void UpdateDynamicNavMeshLink()
    {
        ClearLinks();

        Vector3 extents = transform.lossyScale * 0.5f;
        Vector3 down = -transform.up * extents.y;
        Vector3 right = transform.right * extents.x;
        Vector3 forward = transform.forward * extents.z;

        Vector3 p0 = transform.position + down - right + forward; 
        Vector3 p1 = transform.position + down + right + forward;
        Vector3 p2 = transform.position + down + right - forward;
        Vector3 p3 = transform.position + down - right - forward;

        Vector3[] corners = { p0, p1, p2, p3 };


        for (int i = 0; i < 4; i++)
        {
            Vector3 startCorner = corners[i];
            Vector3 endCorner = corners[(i + 1) % 4];

            float edgeLength = Vector3.Distance(startCorner, endCorner);
            int steps = Mathf.Max(1, Mathf.CeilToInt(edgeLength / LINK_STEP_SIZE));

            for (int j = 0; j < steps; j++)
            {
                Vector3 edgePoint = Vector3.Lerp(startCorner, endCorner, (float)j / steps);
                Vector3 platformSearchPos = edgePoint + (transform.up * (extents.y * 2f));
                Vector3 mapSearchPos = edgePoint - (transform.up * navLinkRange * 0.5f);

                if (NavMesh.SamplePosition(platformSearchPos, out NavMeshHit platformHit, navLinkRange, NavMesh.AllAreas))
                {
                    if (NavMesh.SamplePosition(mapSearchPos, out NavMeshHit mapHit, navLinkRange, NavMesh.AllAreas))
                    {
                        if (Vector3.Distance(platformHit.position, mapHit.position) > 1.0f)
                        {
                            NavMeshLinkData linkData = new NavMeshLinkData();
                            linkData.startPosition = platformHit.position;
                            linkData.endPosition = mapHit.position;
                            linkData.area = 0; 
                            linkData.bidirectional = true;
                            linkData.costModifier = -1f;

                            activeLinks.Add(NavMesh.AddLink(linkData));
                        }
                    }
                }
            }
        }
    }

    private void ClearLinks()
    {
        foreach (var link in activeLinks)
        {
            if (link.valid) link.Remove();
        }
        activeLinks.Clear();
    }

    private void OnDisable()
    {
        ClearLinks();
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }
}
using UnityEngine;

public class FallTrap : MonoBehaviour
{
    [Tooltip("떨어졌을 때 부활할 안전 지대(세이프 존) 트랜스폼")]
    public Transform safeZone;

    [Header("Trap Direction Settings")]
    [Tooltip("기준 아래 벡터")]
    public Vector3 activeDirection = Vector3.down;

    [Tooltip("활성화 여부를 결정할 각도 오차 허용 범위 (도 단위)")]
    public float angleTolerance = 10f;

    private Collider trapCollider;
    private MapManager mapManager;

    private void Awake()
    {
        trapCollider = GetComponent<Collider>();
        mapManager = FindObjectOfType<MapManager>();
    }

    private void Update()
    {
        if (trapCollider == null || mapManager == null || mapManager.map == null) return;
        Vector3 currentWorldDir = mapManager.map.rotation * activeDirection.normalized;
        float angle = Vector3.Angle(currentWorldDir, Vector3.down);
        bool shouldBeActive = angle <= angleTolerance;

        if (trapCollider.enabled != shouldBeActive)
        {
            trapCollider.enabled = shouldBeActive;
        }
    }
}
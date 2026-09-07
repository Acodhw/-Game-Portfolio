using UnityEngine;
using Unity.AI.Navigation; 

[RequireComponent(typeof(NavMeshSurface))]
public class RealtimeNavMeshUpdater : MonoBehaviour
{
    private NavMeshSurface surface;

    [Tooltip("갱신 주기 (초 단위). 너무 짧으면 렉이 걸릴 수 있습니다. 0.1~0.15초 권장")]
    public float updateInterval = 0.15f;
    private float timer = 0f;

    void Start()
    {
        surface = GetComponent<NavMeshSurface>();

        // 시작 시 해당 발판 구조물만 최초 1회 베이킹 수행
        surface.BuildNavMesh();
    }

    void Update()
    {
        // 최적화를 위해 매 프레임이 아닌 주기적으로 갱신합니다.
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;

            // BuildNavMesh()는 렉이 심하므로, 이미 구워진 데이터(navMeshData)를 
            // 현재 이동/회전된 오브젝트의 위치에 맞춰 가볍게 동기화(Update)만 합니다.
            if (surface.navMeshData != null)
            {
                surface.UpdateNavMesh(surface.navMeshData);
            }
        }
    }
}
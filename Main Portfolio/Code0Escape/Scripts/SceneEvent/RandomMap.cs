using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.ComponentModel;
using Unity.Burst.CompilerServices;

public class RandomMap : MonoBehaviour
{
    [SerializeField]
    private GameObject[] x1Maps;
    [SerializeField]
    private GameObject[] x1BridgeMaps;
    [SerializeField] 
    private GameObject[] x2Maps;
    [SerializeField]
    private GameObject finishMap;

    [SerializeField]
    private int maxFindingMaps;

    [SerializeField]
    private float mapMovement;
    [SerializeField]
    private int mapCount;

    Vector3 n;
    Vector3 t;

    private void Start()
    {
        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }


    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void MakeNextMap(Vector3 vectorMapPos, Vector3 nowMapPoint) {
        n = nowMapPoint;
        t = vectorMapPos;
        if (mapCount == maxFindingMaps)
        {
            GameObject g = Instantiate(finishMap, nowMapPoint + vectorMapPos * mapMovement * 0.5f, Quaternion.Euler(-90, 0, 0), transform);
            for (int i = 0; i < g.GetComponent<MapChaker>().doorList.Length; i++)
            {
                g.GetComponent<MapChaker>().doorList[i] = false;
            }
            mapCount++;
        }
        else
        {
            int index = -1;
            do
            {
                index = Random.Range(0, x1Maps.Length + x1BridgeMaps.Length + x2Maps.Length);
            } while ((index >= 0 && index < x1Maps.Length && x1Maps.Length == 0) || (index >= x1Maps.Length && index < x1BridgeMaps.Length + x1Maps.Length && x1BridgeMaps.Length == 0) || (index >= x1BridgeMaps.Length + x1Maps.Length && index < x1BridgeMaps.Length + x1Maps.Length + x2Maps.Length && x2Maps.Length == 0));

            if (index >= 0 && index < x1Maps.Length) index = 0;
            else if (index >= x1Maps.Length && index < x1BridgeMaps.Length + x1Maps.Length) index = 1;
            else index = 2;

            int minRoadCount = 0;
            int maxRoadCount = 0;
            int dcount = 0;
            int rotated = 0;
            GameObject g = null;
            switch (index)
            {
                case 0:
                    maxRoadCount = 4;
                    minRoadCount = 2;
                    dcount = 0;
                    index = Random.Range(0, x1Maps.Length);
                    rotated = Random.Range(0, 4);
                    g = Instantiate(x1Maps[index], nowMapPoint + vectorMapPos * mapMovement * 0.5f, Quaternion.Euler(-90, rotated * 90, 0), transform);
                    for (int i = 0; i < g.GetComponent<MapChaker>().doorList.Length; i++)
                    {
                        if (minRoadCount > dcount)
                        {
                            bool tmp = (Random.Range(i, 4) >= minRoadCount);
                            g.GetComponent<MapChaker>().doorList[i] = tmp;
                            if(tmp) dcount++;
                        }
                        else {
                            g.GetComponent<MapChaker>().doorList[i] = (Random.Range(0, 2) == 0);
                        }
                    }
                    for (int j = 5; j < g.transform.childCount; j++)
                    {
                        g.transform.GetChild(5).parent = null;
                    }
                    break;
                case 1:
                    index = Random.Range(0, x1BridgeMaps.Length);
                    rotated = Random.Range(0, 2);
                    rotated *= 2;
                    g = Instantiate(x1BridgeMaps[index], nowMapPoint + vectorMapPos * mapMovement * 0.5f, Quaternion.Euler(-90, (rotated + ((vectorMapPos.x <= -0.009 || vectorMapPos.x >= 0.009) ? 1 : 0)) * 90, 0), transform);
                    for (int j = 3; j < g.transform.childCount; j++)
                    {
                        g.transform.GetChild(3).parent = null;
                    }

                    break;
                case 2:
                    maxRoadCount = 6;
                    minRoadCount = 3;
                    dcount = 0;
                    index = Random.Range(0, x1BridgeMaps.Length);
                    rotated = Random.Range(2, 4);
                    bool check2x = false;
                    switch (rotated)
                    {
                        case 0:
                        case 1:
                            check2x = Physics.BoxCast(nowMapPoint + mapMovement * 0.5f * vectorMapPos, new Vector3(150, 150, 150) * 0.5f, vectorMapPos, transform.rotation, mapMovement, LayerMask.NameToLayer("Ground"));
                            break;
                        case 2:
                        case 3:
                            check2x = Physics.BoxCast(n + mapMovement * 0.5f * t, new Vector3(150, 150, 150) * 0.5f, Quaternion.Euler(0, 90, 0) * t, transform.rotation, mapMovement, LayerMask.NameToLayer("Ground"));
                            break;
                        case 4:
                        case 5:
                            check2x = Physics.BoxCast(n + mapMovement * 0.5f * t, new Vector3(150, 150, 150) * 0.5f, Quaternion.Euler(0, -90, 0) * t, transform.rotation, mapMovement, LayerMask.NameToLayer("Ground"));
                            break;
                    }
                    if (check2x)
                    {
                        maxRoadCount = 4;
                        minRoadCount = 2;
                        index = Random.Range(0, x1Maps.Length);
                        rotated = Random.Range(0, 4);
                        g = Instantiate(x1Maps[index], nowMapPoint + vectorMapPos * mapMovement * 0.5f, Quaternion.Euler(-90, rotated * 90, 0));
                        for (int j = 5; j < g.transform.childCount; j++)
                        {
                            g.transform.GetChild(5).parent = null;
                        }
                    }
                    else
                    {
                        index = Random.Range(0, x2Maps.Length);
                        switch (rotated)
                        {
                            case 0:
                            case 1:
                                rotated %= 2;
                                g = Instantiate(x2Maps[index], nowMapPoint + vectorMapPos * mapMovement * 1f, Quaternion.Euler(-90, ((vectorMapPos.x <= -0.009 || vectorMapPos.x >= 0.009) ? 0 : 1) * 90 + rotated * 180, 0), transform);
                                break;
                            case 2:
                            case 3:
                                rotated %= 2;
                                g = Instantiate(x2Maps[index], nowMapPoint + vectorMapPos * mapMovement * 0.5f + Quaternion.Euler(0, 90, 0) * vectorMapPos * mapMovement * 0.5f, Quaternion.Euler(-90, ((vectorMapPos.x <= -0.009 || vectorMapPos.x >= 0.009) ? 1 : 0) * 90 + rotated * 180, 0), transform);
                                break;
                            case 4:
                            case 5:
                                rotated %= 2;
                                g = Instantiate(x2Maps[index], nowMapPoint + vectorMapPos * mapMovement * 0.5f + Quaternion.Euler(0, 90, 0) * vectorMapPos * mapMovement * 0.5f, Quaternion.Euler(-90, ((vectorMapPos.x <= -0.009 || vectorMapPos.x >= 0.009) ? 1 : 0) * 90 + rotated * 180, 0), transform);
                                break;
                        }
                        for (int j = 7; j < g.transform.childCount; j++)
                        {
                            g.transform.GetChild(7).parent = null;
                        }
                    }
                    for (int i = 0; i < g.GetComponent<MapChaker>().doorList.Length; i++)
                    {
                        if (minRoadCount > dcount)
                        {
                            bool tmp = (Random.Range(i, maxRoadCount) >= minRoadCount);
                            g.GetComponent<MapChaker>().doorList[i] = tmp;
                            if (tmp) dcount++;
                        }
                        else
                        {
                            g.GetComponent<MapChaker>().doorList[i] = (Random.Range(0, 2) == 0);
                        }
                    }                    
                    break;
            }
            mapCount++;
            if (mapCount > maxFindingMaps)
            {
                for (int i = 0; i < g.GetComponent<MapChaker>().doorList.Length; i++)
                {
                    g.GetComponent<MapChaker>().doorList[i] = false;
                }
            }
            
        }
        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }


    /*void OnDrawGizmos()
    {
        RaycastHit hit;
        
        // Physics.BoxCast (레이저를 발사할 위치, 사각형의 각 좌표의 절판 크기, 발사 방향, 충돌 결과, 회전 각도, 최대 거리)
        bool check2x  = Physics.BoxCast(n + mapMovement * 0.5f * t, new Vector3(150, 150, 150) * 0.5f, Quaternion.Euler(0, 90, 0) * t, out hit, transform.rotation, mapMovement, LayerMask.NameToLayer("Ground"));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(n + mapMovement * 0.5f * t, new Vector3(25, 25, 25));
        if (check2x)
        {
            Gizmos.DrawRay(n + mapMovement * 0.5f * t, Quaternion.Euler(0, 90, 0) * t * hit.distance);
            Gizmos.DrawWireCube(n + mapMovement * 0.5f * t + Quaternion.Euler(0, 90, 0) * t * hit.distance, new Vector3(150, 150, 150));
        }
        else
        {
            Gizmos.DrawRay(n + mapMovement * 0.5f * t, Quaternion.Euler(0, 90, 0) * t * mapMovement * 1f);
        }
    }*/

}



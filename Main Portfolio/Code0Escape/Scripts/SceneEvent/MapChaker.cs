using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MapKind {
    x1Map,
    x1MapBridge,
    x2Map,
}

public class MapChaker : MonoBehaviour
{
    public MapKind mapKind;
    public bool[] doorList;
    public Material road;

    public bool DoorMakeMap = false;

    private RandomMap randomMap;

    // Start is called before the first frame update
    void Start()
    {
        randomMap = GameObject.Find("MapChanger").GetComponent<RandomMap>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < doorList.Length; i++) {
            if (doorList[i]) {
                transform.GetChild(i).GetComponent<MeshRenderer>().material = road;
            }
        }
    }

    public void DoorChecking(int index) {
        if (doorList[index])
        {
            Vector3 nextRoomvec = -transform.GetChild(index).up;
            randomMap.MakeNextMap(nextRoomvec, transform.GetChild(index).position);
            transform.GetChild(index).gameObject.SetActive(!doorList[index]);
        }
    }

    public void closeDoor(Vector3 beforeVec) {
        
    }
}

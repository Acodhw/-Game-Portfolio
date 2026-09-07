using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorChecker : MonoBehaviour
{
    public int doorIndex;
    public Transform myColl;
    MapChaker map;

    void Start()
    {
        map = transform.parent.GetComponent<MapChaker>();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag.Equals("Map") && (hit.transform != myColl))
        {
            gameObject.SetActive(false);
            map.DoorMakeMap = true;
        }
        else if (hit.tag.Equals("Player"))
        {
            if (map.doorList[doorIndex] && map.DoorMakeMap)
            {
                map.DoorChecking(doorIndex);
                gameObject.SetActive(false);
            }
            else if (map.doorList[doorIndex])
            {
                map.DoorMakeMap = true;
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Collider>().isTrigger = map.doorList[doorIndex];
    }
}

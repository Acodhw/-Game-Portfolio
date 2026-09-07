using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPositionChaker : MonoBehaviour
{
    public bool isMapHere = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3) isMapHere = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3) isMapHere = false;
    }
}

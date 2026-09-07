using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFinish : MonoBehaviour
{
    [SerializeField] private GameObject CannonFisishObj;
    // Start is called before the first frame update
    private void OnDestroy()
    {
        Instantiate(CannonFisishObj, transform.position, transform.rotation);
    }
}

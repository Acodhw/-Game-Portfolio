using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSecondObj : MonoBehaviour
{
    public float second;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("remove");
    }

    IEnumerator remove()
    {
        yield return new WaitForSeconds(second);
        Destroy(gameObject);
    }
}

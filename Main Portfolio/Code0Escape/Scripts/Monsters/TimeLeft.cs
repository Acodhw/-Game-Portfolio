using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeft : MonoBehaviour
{
    [SerializeField] private float LeftTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DestroyLeft");
    }

    IEnumerator DestroyLeft() {
        yield return new WaitForSeconds(LeftTime);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpMoveTile : MonoBehaviour
{
    public Vector2 toMove;
    public float time;
    [HideInInspector]
    public int i = 1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("ichange");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(toMove * i * Time.deltaTime);

    }

    IEnumerator ichange() {
        yield return new WaitForSeconds(time);
        i = -1;
        yield return new WaitForSeconds(time);
        i = 1;
        StartCoroutine("ichange");
    }
}

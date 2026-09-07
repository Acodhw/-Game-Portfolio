using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBarrior : MonoBehaviour
{
    float runningTime;
    float radius = 3;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        runningTime += Time.deltaTime * 4;
        float x = radius * Mathf.Cos(runningTime);
        float y = radius * Mathf.Sin(runningTime);
        this.transform.localPosition = new Vector2(x, y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SateliteMove : MonoBehaviour
{
    [SerializeField] private float multi;
    float angle;

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(Mathf.Cos(angle) * 3, Mathf.Sin(angle) * 3, -0.1f);
        angle += Time.deltaTime * multi;
    }
}

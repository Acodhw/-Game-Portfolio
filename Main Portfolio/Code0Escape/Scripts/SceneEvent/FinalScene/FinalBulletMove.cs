using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBulletMove : MonoBehaviour
{
    Rigidbody body;
    public float speed;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = Vector3.down * speed;
    }
}

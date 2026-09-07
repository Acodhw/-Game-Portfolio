using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWoodShot : MonoBehaviour
{
    ShotsEvent se;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        se = GetComponent<ShotsEvent>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity -= se.shotDirection * Time.deltaTime * se.shotSpeed * 1.2f;
    }
}

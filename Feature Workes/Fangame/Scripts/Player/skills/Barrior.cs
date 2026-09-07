using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrior : MonoBehaviour
{
    private SpriteRenderer plsr;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        plsr = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>();
        sr.flipX = plsr.flipX;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
            Destroy(collision.gameObject);
    }
}

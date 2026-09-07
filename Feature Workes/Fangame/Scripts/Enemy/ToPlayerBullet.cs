using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPlayerBullet : MonoBehaviour
{
    public bool isFilped;
    SpriteRenderer spr;
    private Transform player;
    bulletTo bt;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        bt = GetComponent<bulletTo>();
        spr = GetComponent<SpriteRenderer>();
        Invoke("Think", 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.position.x > transform.position.x)
            spr.flipX = !isFilped;
        else
            spr.flipX = isFilped;
    }

    void Think()
    {
        Vector2 v = player.position - transform.position;
        float angle = -(Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg + 90) + 180;
        bt.angle = angle;
        Invoke("Think", 1);
    }
}

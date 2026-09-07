using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spini : MonoBehaviour
{
    private Transform player;
    private bulletTo bt;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        bt = GetComponent<bulletTo>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 v = player.position - transform.position;
        float angle = -(Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg + 90) + 180;
        bt.angle = angle + 90;
    }
}
